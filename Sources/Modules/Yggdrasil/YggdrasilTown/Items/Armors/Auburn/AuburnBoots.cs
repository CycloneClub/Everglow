using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Auburn;

[AutoloadEquip(EquipType.Legs)]
public class AuburnBoots : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(6);

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 12;
		Item.value = 2500;
		Item.rare = ItemRarityID.White;
		Item.defense = 3;
	}

	public override void UpdateEquip(Player player)
	{
		player.moveSpeed += 0.06f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<AuburnRodSkeleton>();
		recipe.AddIngredient<RodWing>(8);
		recipe.AddIngredient(ItemID.Leather, 6);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
