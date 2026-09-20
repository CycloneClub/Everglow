using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class StoneCave_Scene : ModTile, ISceneTile
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

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public void AddScene(int i, int j)
	{
		var scene_Close = new TwilightCastle_RoomScene_OverTiles { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, Texture = ModAsset.StoneCave_Scene_Close.Value };
		scene_Close.CustomDraw += DrawStoneCaveOverTile;
		var scene_Background = new TwilightCastle_RoomScene_Background { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, Texture = ModAsset.StoneCave_Scene_Background.Value };
		scene_Background.CustomDraw += DrawStoneCaveBackground;

		Ins.VFXManager.Add(scene_Close);
		Ins.VFXManager.Add(scene_Background);
	}

	public void DrawStoneCaveOverTile(TwilightCastle_RoomScene_OverTiles otD)
	{
		Texture2D tex0 = ModAsset.StoneCave_Scene_Close.Value;

		bool flipH = otD.FlipHorizontally(otD.OriginTilePos.X, otD.OriginTilePos.Y);
		List<Vertex2D> bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(otD.OriginTilePos.X, otD.OriginTilePos.Y, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);
	}

	public void DrawStoneCaveBackground(TwilightCastle_RoomScene_Background bg)
	{
		Texture2D tex0 = ModAsset.StoneCave_Scene_Background.Value;
		Texture2D tex1 = ModAsset.StoneCave_Scene_Far.Value;

		bool flipH = bg.FlipHorizontally(bg.OriginTilePos.X, bg.OriginTilePos.Y);
		List<Vertex2D> bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(bg.OriginTilePos.X, bg.OriginTilePos.Y, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);

		bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(bg.OriginTilePos.X, bg.OriginTilePos.Y, tex1, bars, flipH);
		Ins.Batch.Draw(tex1, bars, PrimitiveType.TriangleList);
	}
}
