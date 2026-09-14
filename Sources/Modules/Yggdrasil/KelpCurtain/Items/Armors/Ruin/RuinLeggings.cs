using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.Ruin;

[AutoloadEquip(EquipType.Legs)]
public class RuinLeggings : ModItem
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

		Item.defense = 1;

		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Gray;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.04f;
		player.endurance += 0.02f;
	}
}
