using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.DevilHeart;

[AutoloadEquip(EquipType.Body)]
public class DevilHeartLightBreastPlate : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Armor;

	public const int BuffDuration = 10 * 60; // 10 seconds in frames
	public const int CooldownDuration = 35 * 60; // 35 seconds in frames

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

		Item.defense = 4;
	}

	public override void UpdateEquip(Player player)
	{
		player.statManaMax2 += 60; // Increases maximum mana by 60
		player.GetDamage<MagicDamageClass>() += 0.06f; // Increases magic damage by 6%
		player.GetDamage<SummonDamageClass>() += 0.06f; // Increases summon damage by 6%
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return (head.type == ModContent.ItemType<DevilHeartHairpin>() || head.type == ModContent.ItemType<DevilHeartHelmet>())
			&& legs.type == ModContent.ItemType<DevilHeartLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetModPlayer<YggdrasilPlayer>().devilHeartSet = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ModContent.ItemType<DevilHeartIronBar_Item>(), 32)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
