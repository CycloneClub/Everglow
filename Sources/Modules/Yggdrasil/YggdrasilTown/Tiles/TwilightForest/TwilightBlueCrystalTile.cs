namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public abstract class TwilightBlueCrystalTile : ModTile
{
	// At 75 pick power this deals 30 tile damage, matching the Cyan Vine pickaxe on its clusters.
	internal const float MiningResistance = 2.5f;

	internal static Item CreateCrystalDrop() => new(ModContent.ItemType<Items.Materials.TwilightCrystal>(), Main.rand.Next(3, 6));

	public override void SetStaticDefaults()
	{
		MinPick = 75;
		MineResist = MiningResistance;
		RegisterItemDrop(ModContent.ItemType<Items.Materials.TwilightCrystal>());
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return CreateCrystalDrop();
	}
}
