using System.Reflection;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Quests.MainStory;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SubworldLibrary;
using Terraria.WorldBuilding;

namespace Everglow.Yggdrasil;

public class YggdrasilWorld : Subworld
{
	public readonly struct TwilightBubble
	{
		public Vector2 Center { get; }

		public float Radius { get; }

		public TwilightBubble(Vector2 center, float radius)
		{
			Center = center;
			Radius = radius;
		}
	}

	public static Point KelpCurtain_IsleOfBloom_CaveCenter;
	public static readonly List<TwilightBubble> TwilightBubbles = new List<TwilightBubble>();

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
		if (!PlayerQuestManager.Instance.Quests.Contains(new TownAccessCard_Quest()))
		{
			PlayerQuestManager.Instance.AddQuest(new TownAccessCard_Quest(), PlayerQuestState.Available);
		}
	}

	public override void OnLoad()
	{
		Main.worldSurface = Main.maxTilesY - 100;
		Main.rockLayer = Main.maxTilesY - 50;
		YggdrasilTownBiome.CheckedBiomeCenter = false;
		YggdrasilTownBiome.BiomeCenter = YggdrasilTownBiome.GetBiomeCenter();
	}

	public override void Load()
	{
		TwilightBubbles.Clear();
		On_WorldGen.setWorldSize += WorldGen_setWorldSize;
		IL_Liquid.Update += SkipUnderworldEvaporation;
		IL_Projectile.CanExplodeTile += AllowUnderworldExplosions;
		IL_Item.DespawnIfMeetingConditions += SkipFallenStarDespawn;
	}

	public override void Unload()
	{
		IL_Liquid.Update -= SkipUnderworldEvaporation;
		IL_Projectile.CanExplodeTile -= AllowUnderworldExplosions;
		IL_Item.DespawnIfMeetingConditions -= SkipFallenStarDespawn;
	}

	/// <summary>
	/// In Yggdrasil, take the same branch as <c>y &lt;= UnderworldLayer</c> so underworld water is not reduced.
	/// </summary>
	private static void SkipUnderworldEvaporation(ILContext il)
	{
		ILCursor cursor = new(il);
		if (!cursor.TryGotoNext(
			i => i.MatchLdarg(0),
			i => i.MatchLdfld(typeof(Liquid), "y"),
			i => i.MatchCall<Main>("get_UnderworldLayer")))
		{
			throw new InvalidOperationException("Water evaporation check was not found in Liquid.Update.");
		}

		Instruction branch = cursor.Instrs[cursor.Index + 3];
		if (!branch.MatchBle(out ILLabel skipEvaporation))
		{
			throw new InvalidOperationException("Water evaporation branch was not found in Liquid.Update.");
		}

		cursor.EmitDelegate<Func<bool>>(() => InYggdrasil);
		cursor.Emit(OpCodes.Brtrue, skipEvaporation);
	}

	/// <summary>
	/// Fallen stars despawn at dawn. In Yggdrasil, skip that check.
	/// </summary>
	private static void SkipFallenStarDespawn(ILContext il)
	{
		ILCursor cursor = new(il);
		if (!cursor.TryGotoNext(
			i => i.MatchLdarg(0),
			i => i.MatchLdfld<Item>(nameof(Item.type)),
			i => i.MatchLdcI4(ItemID.FallenStar),
			i => i.MatchBneUn(out _)))
		{
			throw new InvalidOperationException("Fallen star dawn despawn was not found in Item.DespawnIfMeetingConditions.");
		}

		Instruction branch = cursor.Instrs[cursor.Index + 3];
		if (!branch.MatchBneUn(out ILLabel skipDespawn))
		{
			throw new InvalidOperationException("Fallen star dawn despawn branch was not found in Item.DespawnIfMeetingConditions.");
		}

		cursor.EmitDelegate<Func<bool>>(() => InYggdrasil);
		cursor.Emit(OpCodes.Brtrue, skipDespawn);
	}

	/// <summary>
	/// Before hardmode, a Hellforge at or below the underworld cannot be exploded. In Yggdrasil, take the same branch as <c>y &lt; UnderworldLayer</c>.
	/// </summary>
	private static void AllowUnderworldExplosions(ILContext il)
	{
		ILCursor cursor = new(il);
		if (!cursor.TryGotoNext(
			i => i.MatchLdarg(2),
			i => i.MatchCall<Main>("get_UnderworldLayer"),
			i => i.MatchBlt(out _)))
		{
			throw new InvalidOperationException("Underworld explosion check was not found in Projectile.CanExplodeTile.");
		}

		Instruction branch = cursor.Instrs[cursor.Index + 2];
		if (!branch.MatchBlt(out ILLabel allowExplosion))
		{
			throw new InvalidOperationException("Underworld explosion branch was not found in Projectile.CanExplodeTile.");
		}

		cursor.EmitDelegate<Func<bool>>(() => InYggdrasil);
		cursor.Emit(OpCodes.Brtrue, allowExplosion);
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
			Main.mapMinX = 0;
			Main.mapMinY = 0;
			Main.mapMaxX = Main.maxTilesX;
			Main.mapMaxY = Main.maxTilesY;

			Main.instance.mapTarget = new RenderTarget2D[
				(Main.maxTilesX / Main.textureMaxWidth) + 1,
				(Main.maxTilesY / Main.textureMaxHeight) + 1];
			Main.mapWasContentLost = new bool[Main.instance.mapTarget.GetLength(0), Main.instance.mapTarget.GetLength(1)];
			Main.initMap = new bool[Main.instance.mapTarget.GetLength(0), Main.instance.mapTarget.GetLength(1)];

			Main.instance.TilePaintSystem = new();
			Main.instance.TilesRenderer = new(Main.instance.TilePaintSystem);
			Main.instance.WallsRenderer = new(Main.instance.TilePaintSystem);
		}
		Main.bottomWorld = Main.maxTilesY * 16;
		Main.rightWorld = Main.maxTilesX * 16;
		Main.maxSectionsX = (Main.maxTilesX - 1) / 200 + 1;
		Main.maxSectionsY = (Main.maxTilesY - 1) / 150 + 1;
	}
}
