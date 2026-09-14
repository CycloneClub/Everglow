namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 竹簪子 - identity-only vanity shell (D-18). The design row carries no
/// 伤害 / 价格 / 稀有度 / 效果 cells and its 描述 cell reads 时装 (vanity), which selects the
/// Vanity category. The head-equip art is absent, so no equip slot is declared. Placed in
/// Items/Misc beside the tracked vanity precedent WitheredMask because no Items/Vanity
/// directory exists in the repository.
/// </summary>
public class BambooHairpin : ModItem
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
