using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LightSeeker;

[AutoloadEquip(EquipType.Body)]
public class ConcentratingJacket : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(3);

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 2500;
		Item.rare = ItemRarityID.Green;
		Item.defense = 5;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetCritChance(DamageClass.Generic) += 3f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LightBulbOvule>(1);
		recipe.AddIngredient(ItemID.Leather, 8);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
