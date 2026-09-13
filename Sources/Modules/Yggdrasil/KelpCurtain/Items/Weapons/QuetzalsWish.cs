namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

/// <summary>
/// 魁札尔的愿望 - designed as a melee giant-blade weapon (design row mel37, 巨刃类武器).
/// The four-stage left-click combo, the charged right-click throw and explosion, and the
/// "wound" debuff all require new projectile/buff/VFX assets that are not in the
/// repository, so the item identity is implemented and the effect is recorded as a blocker.
/// </summary>
public class QuetzalsWish : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
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
