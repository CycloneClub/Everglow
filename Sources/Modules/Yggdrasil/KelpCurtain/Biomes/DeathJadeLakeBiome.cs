using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Background;
using Everglow.Yggdrasil.KelpCurtain.Water;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Biomes;

public class DeathJadeLakeBiome : ModBiome
{
	public static float LiquidSurfaceY = 0;

	public static Dictionary<int, int> RightBoundOfACertainY = [];

	public static void GetLiquidSurfaceY()
	{
		int checkY = (int)(Main.maxTilesY * 0.88f);
		int checkX = Main.maxTilesX / 2;
		for (int j = 0; j < 1000; j++)
		{
			var tile = TileUtils.SafeGetTile(checkX, checkY + j);
			if (tile.LiquidAmount > 0)
			{
				LiquidSurfaceY = (checkY + j) * 16f + 16 - tile.LiquidAmount / 16f; // LiquidAmount 0 ~255
				break;
			}
		}
		RightBoundOfACertainY = [];
		checkY = (int)(LiquidSurfaceY / 16f);
		for (int y = 0; y < 200; y++)
		{
			int checkBoundY = checkY + y;
			int valueX = checkX;
			for (int x = checkX; x < Main.maxTilesX * 0.8f; x++)
			{
				var tileBound = TileUtils.SafeGetTile(x, checkBoundY);
				if (tileBound.HasTile && tileBound.TileType == ModContent.TileType<Tiles.OldMoss>())
				{
					valueX = x;
					break;
				}
			}
			if (RightBoundOfACertainY.ContainsKey(checkBoundY))
			{
				RightBoundOfACertainY.Remove(checkBoundY);
			}
			RightBoundOfACertainY.Add(checkBoundY, valueX + 3);
		}
	}

	/// <summary>
	/// TODO: BGM
	/// </summary>
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.KelpCurtainBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

	public override string BestiaryIcon => ModAsset.DeathJadeLakeIcon_Mod;

	/// <summary>
	/// TODO:Background
	/// </summary>
	public override string BackgroundPath => ModAsset.YggdrasilTown_MapBackground_Mod;

	/// <summary>
	/// TODO:MapBackground
	/// </summary>
	public override string MapBackground => ModAsset.KelpCurtain_MapBackground_Mod;

	/// <summary>
	/// TODO:WaterStyle
	/// </summary>
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<KelpCurtainWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override bool IsBiomeActive(Player player)
	{
		bool flag1 = player.Center.X / 16 > Main.maxTilesX * 0.05f && player.Center.X / 16 < Main.maxTilesX * 0.75f;
		bool flag2 = player.Center.Y / 16 > Main.maxTilesY * 0.87f && player.Center.Y / 16 < Main.maxTilesY * 0.9f;
		bool flag3 = player.InModBiome<KelpCurtainBiome>();
		return flag1 && flag2 && flag3;
	}

	public override void OnInBiome(Player player)
	{
		// Vampire Mat
		if ((Main.LocalPlayer.Center - KelpCurtainGeneration.VampireMatCaveCenter).Length() < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 60 * 16)
		{
			BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
			if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.KelpCurtain.Background.VampireMatCaveWall"))
			{
				List<Vector2> polygon = new List<Vector2>();
				for (int k = 0; k < 40; k++)
				{
					polygon.Add(KelpCurtainGeneration.VampireMatCaveCenter + new Vector2(60 * 16, 0).RotatedBy(k / 40f * MathHelper.TwoPi));
				}
				List<Point> bgArea = TileUtils.GetPolygonAreaOfTilePos(polygon);

				VampireMatCaveWall vmcw = new VampireMatCaveWall();
				vmcw.WorldAnchor = KelpCurtainGeneration.VampireMatCaveCenter;
				vmcw.BgTiles = bgArea;
				bgSystem.AddBackgroundSlide(vmcw);

				VampireMatCaveSky vmcs = new VampireMatCaveSky();
				vmcs.WorldAnchor = KelpCurtainGeneration.VampireMatCaveCenter;
				bgSystem.AddBackgroundSlide(vmcs);
			}
		}

		// Add water light
		float lightTop = LiquidSurfaceY;
		if (lightTop - Main.screenPosition.Y < -Main.offScreenRange)
		{
			lightTop = -Main.offScreenRange + Main.screenPosition.Y;
		}
		float lightBottom = Main.screenPosition.Y + Main.screenHeight + Main.offScreenRange;
		lightBottom = Math.Min(lightBottom, LiquidSurfaceY + 45 * 16f);
		if (lightTop > lightBottom)
		{
			return;
		}
		int yLayers = (int)((lightBottom - lightTop) / 16f);
		for (int offsetY = 0; offsetY < yLayers; offsetY++)
		{
			float rightClamp = Main.screenWidth + Main.offScreenRange + Main.screenPosition.X;
			float rightBound = Main.maxTilesX * 16;
			int tileY = (int)(lightTop / 16) + offsetY;
			if (RightBoundOfACertainY.ContainsKey(tileY))
			{
				int rightX;
				RightBoundOfACertainY.TryGetValue(tileY, out rightX);
				rightBound = rightX;
			}
			if (rightClamp > rightBound)
			{
				rightClamp = rightBound;
			}
			int y = tileY;
			for (int x = (int)((Main.screenPosition.X - Main.offScreenRange) / 16f); x < rightClamp; x++)
			{
				var tile = Main.tile[x, y];
				if (tile.LiquidAmount > 0 && tile.WallType == WallID.None)
				{
					Lighting.AddLight(x, y, 0.1f, 0.4f, 0.3f);
				}
			}
			if (y == LiquidSurfaceY)
			{
				for (int x = (int)((Main.screenPosition.X - Main.offScreenRange) / 16f); x < rightClamp; x++)
				{
					var tile = Main.tile[x, y];
					if (tile.LiquidAmount > 0)
					{
						tile.LiquidAmount = 255;
					}
				}
			}
		}
		base.OnInBiome(player);
	}
}
