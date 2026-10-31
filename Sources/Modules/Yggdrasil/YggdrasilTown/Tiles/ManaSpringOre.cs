namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

// Placeholder tile; world generation and final art are not assigned yet.
public class ManaSpringOre : ModTile
{
	public override string Texture => ModAsset.GreenRelicBrick_Mod;

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		TileID.Sets.Ore[Type] = true;
		MinPick = 90;
		DustType = DustID.Stone;
		HitSound = SoundID.Tink;
		AddMapEntry(new Color(80, 150, 210));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.Materials.ManaSpringOre>());
	}

	public override bool CanExplode(int i, int j) => false;
}
