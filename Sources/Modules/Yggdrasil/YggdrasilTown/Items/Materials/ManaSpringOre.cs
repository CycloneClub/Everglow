namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

// Extremely rare ore reserved for mana progression; world generation is not assigned yet.
public class ManaSpringOre : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Materials;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ManaSpringOre>());
		Item.width = 24;
		Item.height = 24;
		Item.scale = 0.1f;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Orange;
	}
}
