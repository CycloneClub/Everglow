using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using MonoMod.Cil;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Quests.PlayerSides;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace Everglow.Yggdrasil;

public class YggdrasilWorld : Subworld
{
	public static Point KelpCurtain_IsleOfBloom_CaveCenter;

	public static bool InYggdrasil => SubworldSystem.IsActive<YggdrasilWorld>();

	public static float YggdrasilTimer = 0;
	public Vector2 StoneCageOfChallengesCenter = Vector2.zeroVector;

	/// <summary>
	/// After feeding the Giant Ghost Claw Barnacle by the flesh of Vampire Carpet, players can enter the underwater treasury.
	/// </summary>
	public static bool CanEnterTheGiantGhoseClawBarnacle = false;

	public override int Width => 2000;

	public override int Height => 21000;

	public override bool NormalUpdates => true;

	public override bool ShouldSave => false; // Only in debug mode,when published,turn true.

	public override List<GenPass> Tasks => new List<GenPass>()
	{
		new WorldGeneration.YggdrasilWorldGeneration.YggdrasilWorldGenPass(),
	};

	public override void OnEnter()
	{
		SubworldSystem.hideUnderworld = true;
		YggdrasilTimer = 0;
		YggdrasilTownBiome.CheckedBiomeCenter = false;
		if (!PlayerQuestManager.Instance.Quests.Contains(new GetAccessCard()))
		{
			PlayerQuestManager.Instance.AddQuest(new GetAccessCard(), PlayerQuestState.Available);
		}
	}

	public override void OnLoad()
	{
		Main.worldSurface = Main.maxTilesY - 100;
		Main.rockLayer = Main.maxTilesY - 50;
	}

	public override void Load()
	{
		On_WorldGen.setWorldSize += WorldGen_setWorldSize;
		if (!Main.dedServ)
		{
			// 1.4.5 moved the map cache into MapRenderer and fixed it to vanilla world sizes.
			foreach (string method in new[] { "checkMap", "DrawToMap", "CheckMapTargets" })
			{
				Ins.HookManager.AddHook(typeof(MapRenderer).GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), ResizeMapLoops);
			}
		}
	}

	private static void WorldGen_setWorldSize(On_WorldGen.orig_setWorldSize orig)
	{
		int fixedwidth = ((Main.maxTilesX - 1) / 200 + 1) * 200;
		int fixedheight = ((Main.maxTilesY - 1) / 150 + 1) * 150;
		if (fixedwidth + 1 > Main.tile.Width || fixedheight + 1 > Main.tile.Height)
		{
			var createmethod = typeof(Tilemap).GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new Type[] { typeof(ushort), typeof(ushort) });
			Main.tile = (Tilemap)createmethod.Invoke(new object[] { (ushort)(fixedwidth + 1), (ushort)(fixedheight + 1) });
			Main.Map = new(Main.maxTilesX, Main.maxTilesY);
			if (!Main.dedServ)
			{
				EnsureMapCapacity();
				Main.instance.TilePaintSystem = new();
				Main.instance.TilesRenderer = new(Main.instance.TilePaintSystem);
				Main.instance.WallsRenderer = new(Main.instance.TilePaintSystem);
			}
		}
		Main.bottomWorld = Main.maxTilesY * 16;
		Main.rightWorld = Main.maxTilesX * 16;
		Main.maxSectionsX = (Main.maxTilesX - 1) / 200 + 1;
		Main.maxSectionsY = (Main.maxTilesY - 1) / 150 + 1;
	}

	private static void ResizeMapLoops(ILContext il)
	{
		ILCursor cursor = new(il);
		int matches = 0;
		while (cursor.TryGotoNext(i => i.MatchLdsfld<MapRenderer>("numTargetsX") || i.MatchLdsfld<MapRenderer>("numTargetsY")))
		{
			int dimension = ((FieldReference)cursor.Next.Operand).Name == "numTargetsX" ? 0 : 1;
			cursor.Remove();
			cursor.EmitDelegate(() => MapRenderer.mapTarget.GetLength(dimension));
			matches++;
		}
		if (matches == 0)
		{
			throw new InvalidOperationException($"Cannot resize the 1.4.5 map cache: no dimension fields in {il.Method.FullName}.");
		}
	}

	private static void EnsureMapCapacity()
	{
		// Leave one spare edge target: vanilla checkMap shrinks its final row/column.
		int oldWidth = MapRenderer.mapTarget.GetLength(0);
		int oldHeight = MapRenderer.mapTarget.GetLength(1);
		int width = Math.Max(MapRenderer.mapTarget.GetLength(0), Main.maxTilesX / MapRenderer.textureMaxWidth + 2);
		int height = Math.Max(MapRenderer.mapTarget.GetLength(1), Main.maxTilesY / MapRenderer.textureMaxHeight + 2);
		ResizeMapArray(ref MapTargets(null), width, height);
		ResizeMapArray(ref MapInitialized(null), width, height);
		ResizeMapArray(ref MapContentLost(null), width, height);
		ResizeMapArray(ref MapChangeQueues(null), width, height);
		// An old edge texture can be only 400/600 pixels wide/high. Once it becomes
		// an interior target, it must be recreated at the full target dimensions.
		for (int x = 0; x < oldWidth; x++)
		{
			for (int y = 0; y < oldHeight; y++)
			{
				if ((width > oldWidth && x == oldWidth - 1) || (height > oldHeight && y == oldHeight - 1))
				{
					var target = MapRenderer.mapTarget[x, y];
					MapRenderer.mapTarget[x, y] = null;
					MapInitialized(null)[x, y] = false;
					MapContentLost(null)[x, y] = false;
					if (target is not null)
					{
						Main.QueueMainThreadAction(target.Dispose);
					}
				}
			}
		}
		var queues = MapChangeQueues(null);
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				queues[x, y] ??= new List<Point16>();
			}
		}
		Main.clearMap = true;
		Main.refreshMap = true;
	}

	private static void ResizeMapArray<T>(ref T[,] array, int width, int height)
	{
		if (array.GetLength(0) == width && array.GetLength(1) == height)
		{
			return;
		}
		var resized = new T[width, height];
		for (int x = 0; x < array.GetLength(0); x++)
		{
			for (int y = 0; y < array.GetLength(1); y++)
			{
				resized[x, y] = array[x, y];
			}
		}
		array = resized;
	}

	[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "mapTarget")]
	private static extern ref RenderTarget2D[,] MapTargets(MapRenderer declaringType);

	[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "initMap")]
	private static extern ref bool[,] MapInitialized(MapRenderer declaringType);

	[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "mapWasContentLost")]
	private static extern ref bool[,] MapContentLost(MapRenderer declaringType);

	[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "changeQueues")]
	private static extern ref List<Point16>[,] MapChangeQueues(MapRenderer declaringType);
}
