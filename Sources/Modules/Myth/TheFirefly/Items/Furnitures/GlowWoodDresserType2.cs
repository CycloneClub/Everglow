using Terraria.GameContent.Creative;

namespace Everglow.Myth.TheFirefly.Items.Furnitures;

public class GlowWoodDresserType2 : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furnitures.GlowWoodDresserType2>());
		Item.width = 26;
		Item.height = 22;
		Item.value = 500;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<GlowWood>(), 16);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
