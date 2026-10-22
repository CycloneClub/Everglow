using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Twilight;

[AutoloadEquip(EquipType.Body)]
public class TwilightWoodBreastplate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public const int MaxLifeBonus = 15;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(silver: 37, copper: 50);
		Item.rare = ItemRarityID.White;
		Item.defense = 4;
	}

	public override void UpdateEquip(Player player)
	{
		player.statLifeMax2 += MaxLifeBonus;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(30);
		recipe.AddIngredient<TwilightCrystal>(12);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
