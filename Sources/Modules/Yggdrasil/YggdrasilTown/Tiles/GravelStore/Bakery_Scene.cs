using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class Bakery_Scene : ModTile, ISceneTile
{
	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[ModContent.TileType<GreenRelicBrick>()][Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = DustID.BorealWood;
		HitSound = SoundID.Dig;
		MinPick = int.MaxValue;
		AddMapEntry(new Color(63, 47, 38));
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => base.PreDraw(i, j, spriteBatch);

	public void AddScene(int i, int j)
	{
		// TODO: A more general scene_VFX for more wide utilities in lieu of TwilightCastle.
		var scene_Close = new TwilightCastle_RoomScene_OverTiles { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		scene_Close.CustomDraw += DrawBakeryOverTile;

		var scene_Background = new TwilightCastle_RoomScene_Background { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		scene_Background.CustomDraw += DrawBakeryBackground;

		Ins.VFXManager.Add(scene_Close);
		Ins.VFXManager.Add(scene_Background);
	}

	public void DrawBakeryOverTile(TwilightCastle_RoomScene_OverTiles otD)
	{
		bool flipH = false;
		Texture2D tex0 = ModAsset.BakeryRoof.Value;

		var bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(otD.OriginTilePos.X - 1, otD.OriginTilePos.Y - 1, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);
	}

	public void DrawBakeryBackground(TwilightCastle_RoomScene_Background bg)
	{
		bool flipH = false;
		Texture2D tex0 = ModAsset.BakeryWall.Value;

		var bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(bg.OriginTilePos.X, bg.OriginTilePos.Y, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		base.NearbyEffects(i, j, closer);
	}
}
