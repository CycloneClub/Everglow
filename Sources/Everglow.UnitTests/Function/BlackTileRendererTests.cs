using System.Reflection;
using Everglow.Commons.Mechanics.EliminateLight;
using Everglow.SubSpace;
using Everglow.Yggdrasil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.Utilities;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class BlackTileRendererTests
{
	private readonly Stack<Action> restore = new();

	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestCleanup]
	public void CleanUp()
	{
		while (restore.TryPop(out var action))
		{
			action();
		}
		EliminateLightManager.Clear();
	}

	[TestMethod]
	public void NativeDrawBlackKeepsWindowsOpenAcrossSubworldDepths()
	{
		var current = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldWorld = current.GetValue(null);
		var oldScreen = (Main.screenPosition, Main.screenWidth, Main.screenHeight, Main.drawToScreen, Main.offScreenRange, Main.GameViewMatrix);
		var oldLighting = (Lighting._activeEngine, Lighting._mode, Lighting.GlobalBrightness, Main.tileColor, Main.shimmerAlpha);
		var oldScene = (Main.SceneMetrics, LiquidEdgeRenderer.Enabled, TimeLogger.drawTimes, Main.worldSurface, Main.maxTilesY);
		restore.Push(() =>
		{
			current.SetValue(null, oldWorld);
			(Main.screenPosition, Main.screenWidth, Main.screenHeight, Main.drawToScreen, Main.offScreenRange, Main.GameViewMatrix) = oldScreen;
			(Lighting._activeEngine, Lighting._mode, Lighting.GlobalBrightness, Main.tileColor, Main.shimmerAlpha) = oldLighting;
			(Main.SceneMetrics, LiquidEdgeRenderer.Enabled, TimeLogger.drawTimes, Main.worldSurface, Main.maxTilesY) = oldScene;
		});
		Main.worldSurface = 50;
		Main.maxTilesY = 500;
		Main.drawToScreen = true;
		Main.offScreenRange = 0;
		Main.screenWidth = 9 * 16;
		Main.screenHeight = 16;
		Main.GameViewMatrix = new SpriteViewMatrix(null);
		Main.GameViewMatrix.SetViewportOverride(new Viewport(0, 0, Main.screenWidth, Main.screenHeight));
		Main.SceneMetrics = new SceneMetrics { EchoMonolith = true };
		Main.tileColor = Color.White;
		Main.shimmerAlpha = 0;
		TimeLogger.drawTimes = [];
		var engine = new LightingEngine();
		Lighting._activeEngine = engine;
		Lighting._mode = LightMode.Color;
		Lighting.GlobalBrightness = 1f;
		LiquidEdgeRenderer.Enabled = false;
		SetFlag(Main.wallLight, WallID.Glass, true);
		SetFlag(Main.tileSolid, TileID.Stone, true);
		SetFlag(Main.tileBlockLight, TileID.Stone, true);

		const int left = 50;
		int row = 0;
		var runs = new List<Point>();
		var patch = typeof(EliminateLightSystem).GetMethod("PatchDrawBlack", BindingFlags.Static | BindingFlags.NonPublic)!
			.CreateDelegate<ILContext.Manipulator>();
		var method = typeof(Main).GetMethod("DrawBlack", BindingFlags.Instance | BindingFlags.NonPublic)!;
		// Intercept only sprite submission. The installed DrawBlack hook and the native row loop both run.
		using var capture = new ILHook(method, il =>
		{
			patch(il);
			var c = new ILCursor(il);
			c.GotoNext(i => i.MatchLdsfld<Main>(nameof(Main.spriteBatch)), i => i.MatchLdsfld(typeof(TextureAssets), nameof(TextureAssets.BlackTile)));
			c.RemoveRange(3);
			c.GotoNext(i => i.MatchCallvirt<SpriteBatch>(nameof(SpriteBatch.Draw)));
			c.Remove();
			c.EmitDelegate((Vector2 position, Rectangle? source, Color _) =>
			{
				Point cell = (position + Main.screenPosition).ToTileCoordinates();
				if (cell.Y == row)
				{
					runs.Add(new Point(cell.X - left, source!.Value.Width / 16));
				}
			});
		});
		var draw = method.CreateDelegate<On_Main.orig_DrawBlack>();
		foreach (int y in new[] { 30, 280, 310 })
		{
			row = y;
			Main.screenPosition = new Vector2(left, y) * 16;
			for (int i = -1; i < 11; i++)
			{
				var tile = Main.tile[left + i, y];
				Clear<TileTypeData>(tile);
				Clear<WallTypeData>(tile);
				Clear<TileWallWireStateData>(tile);
				Clear<TileWallBrightnessInvisibilityData>(tile);
				Clear<LiquidData>(tile);
				if (i is 0 or 1 or 3)
				{
					tile.WallType = WallID.Stone;
				}
				if (i == 4)
				{
					tile.WallType = WallID.Glass;
				}
				if (i is 5 or 6 or 7)
				{
					tile.HasTile = true;
					tile.TileType = TileID.Stone;
					tile.IsHalfBlock = i == 6;
					if (i == 7)
					{
						tile.Slope = SlopeType.SlopeDownLeft;
					}
				}
			}

			engine._activeProcessedArea = new Rectangle(left, y, 9, 1);
			foreach (Subworld world in new Subworld[] { new YggdrasilWorld(), new RoomWorld() })
			{
				current.SetValue(null, world);
				foreach (var light in new[] { Vector3.Zero, Vector3.One })
				{
					Array.Fill(engine._activeLightMap._colors, light);
					runs.Clear();
					draw(null!, false);
					CollectionAssert.AreEqual(new[] { new Point(0, 2), new Point(3, 1), new Point(5, 1) }, runs);
				}
			}

			current.SetValue(null, null);
			Array.Fill(engine._activeLightMap._colors, Vector3.Zero);
			runs.Clear();
			draw(null!, false);
			if (y == 280)
			{
				Assert.AreEqual(0, runs.Count);
				draw(null!, true);
			}
			Assert.AreEqual(y < Main.UnderworldLayer ? 3 : 1, runs.Count);
		}
	}

	[TestMethod]
	public void EliminationKeepsNeutralMinimumAndLocalEmission()
	{
		var apply = typeof(EliminateLightSystem).GetMethod("On_TileLightScanner_ApplyWallLight", BindingFlags.Instance | BindingFlags.NonPublic)!
			.CreateDelegate<On_TileLightScanner.hook_ApplyWallLight>(new EliminateLightSystem());
		FastRandom random = default;
		static void Emit(TileLightScanner scanner, Tile tile, int x, int y, ref FastRandom rng, ref Vector3 color) => color.Z = Math.Max(color.Z, 0.5f);
		EliminateLightManager.Add(50, 50);
		var light = Vector3.One;
		apply(Emit, null!, default, 50, 50, ref random, ref light);
		Assert.AreEqual(new Vector3(0.01f, 0.01f, 0.5f), light);
		light = new Vector3(1f, 0f, 0.7f);
		apply(Emit, null!, default, 51, 50, ref random, ref light);
		Assert.AreEqual(new Vector3(1f, 0.01f, 0.7f), light);
	}

	private void Clear<T>(Tile tile)
		where T : unmanaged, ITileData
	{
		var old = tile.Get<T>();
		restore.Push(() => tile.Get<T>() = old);
		tile.Get<T>() = default;
	}

	private void SetFlag(bool[] flags, int index, bool value)
	{
		bool old = flags[index];
		restore.Push(() => flags[index] = old);
		flags[index] = value;
	}
}
