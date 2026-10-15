using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class DarkDragon_Scene : ModTile, ISceneTile
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
		Ins.VFXManager.Add(scene_Close);

		var scene_Manager = new DarkDragon_Scene_DustManager { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		Ins.VFXManager.Add(scene_Manager);
	}

	public void DrawOverTile(TwilightCastle_RoomScene_OverTiles otD)
	{
		Texture2D tex0 = ModAsset.DarkDragon_Scene_Background.Value;
		bool flipH = otD.FlipHorizontally(otD.OriginTilePos.X, otD.OriginTilePos.Y);
		Ins.Batch.Draw(tex0, otD.Position, null, Color.White, 0, flipH ? new Vector2(tex0.Width - 16, 0) : Vector2.zeroVector, 1, flipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		base.NearbyEffects(i, j, closer);
	}
}
