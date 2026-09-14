namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 弟子时装 - identity-only vanity shell (D-18/D-19). The design row carries no 伤害 / 击退 /
/// 暴击 / 使用时间 / 其他数值 / 价格 / 稀有度 / 效果 cells, and the item belongs to the 弟子
/// (disciple) progression system, which is not implemented in this phase, so the class is an
/// identity only and its behaviour is blocked on that system. No equip slot is declared and
/// no equip registration is added because no approved vanity art exists. Placed beside
/// <see cref="WitheredMask"/>, the tracked vanity precedent (no Items/Vanity/ directory exists).
/// The approved artwork (贴图) is absent from the repository (D-13).
/// </summary>
public class DiscipleVanity : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Vanity;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
	}
}
