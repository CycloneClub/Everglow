using Everglow.Myth.TheFirefly.Items;

namespace Everglow.Myth.TheFirefly.Tiles;

public class LifeFluorescentTreeWood : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<LifeFluorescentTreeLeaf>()] = true;
		Main.tileMerge[ModContent.TileType<LifeFluorescentTreeLeaf>()][Type] = true;
		DustType = ModContent.DustType<Dusts.FluorescentLeafDust>();
		AddMapEntry(new Color(55, 24, 63));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<GlowWood>());
	}
}
