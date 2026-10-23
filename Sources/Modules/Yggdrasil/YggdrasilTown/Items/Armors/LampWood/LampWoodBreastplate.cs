using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LampWood;

[AutoloadEquip(EquipType.Body)]
public class LampWoodBreastplate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	private const int LifeRegenBonus = 2;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 18;
		Item.value = Item.buyPrice(silver: 20);
		Item.rare = ItemRarityID.Green;
		Item.defense = 3;
	}

	public override void UpdateEquip(Player player)
	{
		player.lifeRegen += LifeRegenBonus;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(30);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
