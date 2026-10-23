using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LampWood;

[AutoloadEquip(EquipType.Legs)]
public class LampWoodLeggings : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	private const int MoveSpeedBonus = 6;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 16;
		Item.value = Item.buyPrice(silver: 20);
		Item.rare = ItemRarityID.Green;
		Item.defense = 2;
	}

	public override void UpdateEquip(Player player)
	{
		player.moveSpeed += MoveSpeedBonus / 100f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(20);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
