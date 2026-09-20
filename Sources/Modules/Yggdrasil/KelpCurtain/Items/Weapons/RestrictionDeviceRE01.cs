namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RestrictionDeviceRE01 : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 18;
		Item.knockBack = 2f;
		Item.mana = 15;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 21;
		Item.noMelee = true;

		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 4);
	}
}
