using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Tiles;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Materials;

public class DevilHeartIronBar_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<DevilHeartIronBar>());
		Item.width = 30;
		Item.height = 24;
		Item.value = 1800;
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<DevilHeartIronOre_Item>(), 3)
			.AddIngredient(ModContent.ItemType<JadeizedBone_Item>(), 1)
			.AddTile(TileID.Furnaces)
			.Register();
	}
}
