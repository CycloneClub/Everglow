using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.DevilHeart;

[AutoloadEquip(EquipType.Head)]
public class DevilHeartHairpin : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
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
		player.GetDamage<MagicDamageClass>() += 0.05f; // Increases magic damage by 5%
		player.GetCritChance<MagicDamageClass>() += 5; // Increases magic critical strike chance by 5%
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<DevilHeartLightBreastPlate>() && legs.type == ModContent.ItemType<DevilHeartLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.manaCost -= 0.1f; // Reduces mana cost by 10%
		player.GetDamage<MagicDamageClass>() += 0.08f; // Increases magic damage by 8%
		player.setBonus = this.GetLocalizedValue(LocalizationUtils.LocalizationKeys.SetBonus);
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<DevilHeartIronBar_Item>(), 18)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
