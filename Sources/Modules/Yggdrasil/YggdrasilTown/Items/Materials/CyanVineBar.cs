using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class CyanVineBar : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CyanVine.CyanVineBar>());
		Item.width = 30;
		Item.height = 24;
		Item.value = 1600;
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<CyanVineOre>(), 3)
			.AddTile(TileID.Furnaces)
			.Register();
	}
}
