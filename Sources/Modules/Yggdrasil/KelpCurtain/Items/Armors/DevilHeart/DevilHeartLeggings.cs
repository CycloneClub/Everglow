using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.DevilHeart;

[AutoloadEquip(EquipType.Legs)]
public class DevilHeartLeggings : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.height = 20;
		Item.width = 20;

		Item.value = Item.buyPrice(0, 0, 60, 0);
		Item.rare = ItemRarityID.Green;

		Item.defense = 3;
	}

	public override void UpdateEquip(Player player)
	{
		player.moveSpeed += 0.1f; // Increases movement speed by 10%
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<DevilHeartIronBar_Item>(), 24)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
