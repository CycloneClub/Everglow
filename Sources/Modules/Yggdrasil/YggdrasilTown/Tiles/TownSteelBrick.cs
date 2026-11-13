using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class TownSteelBrick : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;

		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = true;
		Main.tileShine2[Type] = false;

		DustType = ModContent.DustType<TownSteelBrickDust>();
		MinPick = 100;
		HitSound = SoundID.NPCHit4;

		AddMapEntry(new Color(93, 93, 104));
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var offsetScreen = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offsetScreen = Vector2.Zero;
		}
		Texture2D tex = ModAsset.TownSteelBrick_Glow.Value;
		spriteBatch.Draw(tex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(155, 155, 155, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		base.PostDraw(i, j, spriteBatch);
	}
}
