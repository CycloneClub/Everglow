using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Myth.TheFirefly.Dusts;

namespace Everglow.Myth.TheFirefly.Tiles;

public class LifeFluorescentTreeLeaf : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<LifeFluorescentTreeWood>()] = true;
		Main.tileMerge[ModContent.TileType<LifeFluorescentTreeWood>()][Type] = true;
		TileUtils.Sets.TileFragile[Type] = true;
	}

	public override void PostSetDefaults()
	{
		DustType = ModContent.DustType<FluorescentLeafDust>();
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(0, 53, 158));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.LifeFluorescentTreeLeaf_Glow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(55, 55, 55, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		base.PostDraw(i, j, spriteBatch);
	}
}
