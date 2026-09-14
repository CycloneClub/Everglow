using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.Ruin;

[AutoloadEquip(EquipType.Body)]
public class RuinMagicRobe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 18;

		Item.defense = 3;

		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Gray;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.06f;
		player.manaCost *= 1 - 0.1f;
	}
}
