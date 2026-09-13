namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 枯萎面具 - vanity item (时装道具) with no combat stats, traded with Withered Wood
/// chunks. The head-equip art (`WitheredMask_Head.png`) is not in the repository, so the
/// equip slot is not autoloaded yet and the artwork is recorded as a blocker.
/// </summary>
public class WitheredMask : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Vanity;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.value = Item.buyPrice(silver: 15);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
	}
}
