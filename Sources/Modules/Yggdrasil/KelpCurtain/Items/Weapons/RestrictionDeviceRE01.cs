namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RestrictionDeviceRE01 : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
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

	// Effect blocker: the design summons a 限制无人机 that flies fast and fires a 聚能射线
	// which splits its damage evenly across multiple locked targets, dealing full damage to a
	// single locked target and accumulating 浊燃 damage. No such summon projectile or beam
	// exists in the repository, so Item.shoot is deliberately not set.
	//
	// Recipe blocker: the design 合成方式 cell lists 血云母 + 血肉聚合物 + 熔炉钢 + 隐生之眼,
	// all Phase 7 Giant Winged Dragon items absent from the repository; referencing an absent
	// type is a compile error, so no AddRecipes body is written.
}
