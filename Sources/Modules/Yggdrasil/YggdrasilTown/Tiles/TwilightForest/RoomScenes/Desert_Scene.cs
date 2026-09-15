using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class Desert_Scene : ModTile, ISceneTile
{
	public override string Texture => ModAsset.GreenRelicBrick_Mod;

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkForestSoil>()] = true;
		Main.tileMerge[Type][ModContent.TileType<StoneScaleWood>()] = true;

		Main.tileMerge[Type][ModContent.TileType<GreenRelicBrick>()] = true;
		Main.tileMerge[ModContent.TileType<GreenRelicBrick>()][Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<GreenRelicBrick_dust>();
		HitSound = SoundID.Dig;
		MinPick = int.MaxValue;
		AddMapEntry(new Color(35, 58, 58));
	}

	public bool FlipHorizontally(int i, int j)
	{
		int leftSolid = 0;
		int rightSolid = 0;
		for (int x = 1; x < 15; x++)
		{
			Tile tileLeft = TileUtils.SafeGetTile(i - x, j);
			Tile tileRight = TileUtils.SafeGetTile(i + x, j);
			if (tileLeft.HasTile && tileLeft.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				leftSolid++;
			}
			if (tileRight.HasTile && tileRight.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				rightSolid++;
			}
		}
		if (rightSolid > leftSolid)
		{
			return false;
		}
		return true;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public void AddScene(int i, int j)
	{
		var scene_Close = new TwilightCastle_RoomScene_OverTiles { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		scene_Close.CustomDraw += DrawOverTile;

		var scene_Background = new TwilightCastle_RoomScene_Background { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		scene_Background.CustomDraw += DrawBackground;

		Ins.VFXManager.Add(scene_Close);
		Ins.VFXManager.Add(scene_Background);
	}

	public void DrawOverTile(TwilightCastle_RoomScene_OverTiles otD)
	{
		Texture2D tex0 = ModAsset.Desert_Scene_Close.Value;
		bool flipH = otD.FlipHorizontally(otD.OriginTilePos.X, otD.OriginTilePos.Y);
		int direction = 1;
		if (flipH)
		{
			direction = -1;
		}

		Tile tile_lantern = TileUtils.SafeGetTile(otD.OriginTilePos + new Point(7 * direction, 5));
		if (tile_lantern.TileType == ModContent.TileType<SandgoldLantern>() && tile_lantern.TileFrameX == 0)
		{
			tex0 = ModAsset.Desert_Scene_Close_lightUp.Value;
		}

		List<Vertex2D> bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(otD.OriginTilePos.X, otD.OriginTilePos.Y, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);
	}

	public void DrawBackground(TwilightCastle_RoomScene_Background bg)
	{
		Texture2D tex0 = ModAsset.Desert_Scene_Background.Value;
		Texture2D tex1 = ModAsset.Desert_Scene_Far.Value;
		Texture2D tex2 = ModAsset.Desert_Scene_WallGemsReflection.Value;

		bool flipH = bg.FlipHorizontally(bg.OriginTilePos.X, bg.OriginTilePos.Y);
		int direction = 1;
		if (flipH)
		{
			direction = -1;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(bg.OriginTilePos.X, bg.OriginTilePos.Y, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);

		bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(bg.OriginTilePos.X, bg.OriginTilePos.Y, tex1, bars, flipH);
		Ins.Batch.Draw(tex1, bars, PrimitiveType.TriangleList);

		bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(bg.OriginTilePos.X + 34 * direction, bg.OriginTilePos.Y + 11, tex2, bars, flipH, 2.6f);
		Ins.Batch.Draw(tex2, bars, PrimitiveType.TriangleList);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (!Main.gamePaused)
		{
			bool flipH = FlipHorizontally(i, j);
			int direction = 1;
			if (flipH)
			{
				direction = -1;
			}
			if (Main.rand.NextBool(160))
			{
				var spark = new Desert_Scene_WallGemsSpark { Position = new Vector2(i, j) * 16 + new Vector2(575 * direction, 209) + new Vector2(Main.rand.NextFloat(13), Main.rand.NextFloat(13)), Active = true, Visible = true, Frame = new Rectangle(0, Main.rand.Next(3) * 14, 14, 14), Scale = Main.rand.NextFloat(0.75f, 1.25f), Color = new Color(0f, 1f, 0.4f), MaxTime = 20, Fade = Main.rand.NextFloat(0.25f, 0.75f) };
				Ins.VFXManager.Add(spark);
			}
			for (int k = 0; k < 8; k++)
			{
				if (Main.rand.NextBool(480))
				{
					Vector2 offset = new Vector2(22, 0).RotatedBy(k / 8f * MathHelper.TwoPi);
					Color color = new Color(1f, 0f, 0.1f);
					if (k % 2 == 1)
					{
						color = new Color(0f, 0f, 1f);
					}
					var spark = new Desert_Scene_WallGemsSpark { Position = new Vector2(i, j) * 16 + new Vector2(575 * direction, 209) + offset + new Vector2(Main.rand.NextFloat(13), Main.rand.NextFloat(13)), Active = true, Visible = true, Frame = new Rectangle(0, Main.rand.Next(3) * 14, 14, 14), Scale = Main.rand.NextFloat(0.75f, 1.25f), Color = color, MaxTime = 20, Fade = Main.rand.NextFloat(0.25f, 0.75f) };
					Ins.VFXManager.Add(spark);
				}
			}
		}
		base.NearbyEffects(i, j, closer);
	}
}
