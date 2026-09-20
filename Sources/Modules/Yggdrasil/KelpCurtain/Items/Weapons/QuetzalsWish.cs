namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class QuetzalsWish : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 70;
		Item.height = 70;
		Item.damage = 37;
		Item.DamageType = DamageClass.Melee;
		Item.crit = 8;
		Item.knockBack = 6f;
		Item.useTime = 24;
		Item.useAnimation = 24;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.autoReuse = true;
		Item.UseSound = SoundID.Item1;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 10);
	}
}
