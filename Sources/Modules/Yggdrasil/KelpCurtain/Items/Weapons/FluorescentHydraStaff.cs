namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

/// <summary>
/// 荧光水螅召唤杖 - identity-only shell (D-18). The design row carries no 伤害 / 击退 /
/// 暴击 / 使用时间 / 其他数值 / 价格 / 稀有度 / 效果 cells, and the 荧光水螅 summon projectile
/// and its behaviour are not in the repository, so no Item.shoot and no Item.staff
/// declaration is added and the class is an identity only. The approved artwork (贴图)
/// is absent from the repository (D-13).
/// </summary>
public class FluorescentHydraStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
	}
}
