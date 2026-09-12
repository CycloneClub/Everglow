using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.DevilHeart;

[AutoloadEquip(EquipType.Head)]
public class DevilHeartHelmet : ModItem
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

		Item.defense = 1;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.04f; // Increases summon damage by 4%
		player.slotsMinions += 1; // Increases the number of minions the player can summon by 1
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<DevilHeartLightBreastPlate>() && legs.type == ModContent.ItemType<DevilHeartLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.08f; // Increases summon damage by 8%
		player.slotsMinions += 1; // Increases the number of minions the player can summon by 1
		player.GetAttackSpeed<SummonDamageClass>() += 0.15f; // Increases summon attack speed by 15%
		player.setBonus = this.GetLocalizedValue(LocalizationUtils.LocalizationKeys.SetBonus);
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<DevilHeartIronBar_Item>(), 20)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
