using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.CyanVine;

[AutoloadEquip(EquipType.Body)]
public class CyanBreastplate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(5, 0.05f);

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 3750;
		Item.rare = ItemRarityID.Green;
		Item.defense = 5;
	}

	public override void UpdateEquip(Player player)
	{
		player.jumpSpeedBoost += 0.05f;
		player.moveSpeed += 0.05f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<CyanVineBar>(25);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
