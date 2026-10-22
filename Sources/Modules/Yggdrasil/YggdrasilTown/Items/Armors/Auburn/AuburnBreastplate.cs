using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Auburn;

[AutoloadEquip(EquipType.Body)]
public class AuburnBreastplate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 18;
		Item.value = 2500;
		Item.rare = ItemRarityID.White;
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player)
	{
		player.whipRangeMultiplier += 0.10f;
		player.summonerWeaponSpeedBonus += 0.05f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<RodWing>(16);
		recipe.AddIngredient(ItemID.Leather, 8);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
