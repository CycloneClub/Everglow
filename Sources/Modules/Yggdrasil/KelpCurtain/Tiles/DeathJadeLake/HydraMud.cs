using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class HydraMud : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkLakeBottomMud>()] = true;
		Main.tileMerge[ModContent.TileType<DarkLakeBottomMud>()][Type] = true;
		DustType = ModContent.DustType<HydraMud_Dust>();
		AddMapEntry(new Color(34, 43, 39));
	}

	public void AddScene(int i, int j)
	{
		HydraMudTentacles_fore leaf = new HydraMudTentacles_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		leaf.scale = 1f;
		leaf.style = (i + j) % 4;
		Ins.VFXManager.Add(leaf);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Tile tile = Main.tile[i, j];
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Vector2 pos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
		spriteBatch.Draw(tex, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY + 90, 16, 16), new Color(0.3f, 0.3f, 0.3f, 0));
	}
}
