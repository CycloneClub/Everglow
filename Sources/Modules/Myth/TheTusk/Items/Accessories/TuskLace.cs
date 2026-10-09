namespace Everglow.Myth.TheTusk.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class TuskLace : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Tusk Necklace");
		// //Tooltip.SetDefault("Increases armor penetration by 5\nIncreases damage and crit chance by 5%\n'A tusk was knocked down from mouth of a beast, again you knocked down a small part of it'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "盔甲穿透增加5\n伤害和暴击增加5%\n'一颗獠牙被人从某只猛兽嘴里打了下来,你又敲下了上面的一小部分'");
	}

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.value = 2000;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		// Item.vanity = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 5;
		player.GetCritChance(DamageClass.Generic) += 5;
		player.GetDamage(DamageClass.Generic) *= 1.05f;
	}
}
