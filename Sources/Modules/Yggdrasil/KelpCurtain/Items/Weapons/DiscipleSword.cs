namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

/// <summary>
/// 弟子剑 - identity-only shell (D-18/D-19). The design row carries no 伤害 / 击退 / 暴击 /
/// 使用时间 / 其他数值 / 价格 / 稀有度 / 效果 cells, and the item belongs to the 弟子 (disciple)
/// progression system, which is not implemented in this phase, so the class is an identity
/// only and its behaviour is blocked on that system. The approved artwork (贴图) is absent
/// from the repository (D-13).
/// </summary>
public class DiscipleSword : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = 1;
	}
}
