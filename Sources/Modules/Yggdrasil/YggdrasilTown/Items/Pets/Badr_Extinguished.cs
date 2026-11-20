using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Pets;

public sealed class Badr_Extinguished : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Pets;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 32;
		Item.maxStack = 1;
		Item.rare = ItemRarityID.Quest;
		Item.questItem = true;
	}

	public override void AddRecipes()
	{
		Recipe.Create(ModContent.ItemType<Badr>())
			.AddIngredient(Type)
			.AddTile<GiantFurnace>()
			.Register();
	}
}
