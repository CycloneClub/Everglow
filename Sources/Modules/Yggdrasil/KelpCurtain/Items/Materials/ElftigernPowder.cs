namespace Everglow.Yggdrasil.KelpCurtain.Items.Materials;

/// <summary>
/// 厄佛提根的净化粉末 - designed to purify a Wilted Zone (枯萎地带) and traded with
/// Withered Wood chunks. The restoration system it feeds is GAME-03/Phase 6, so the
/// item identity is implemented but the purification hook is not yet available.
/// </summary>
public class ElftigernPowder : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 24;
		Item.value = Item.buyPrice(silver: 25);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 15;
		Item.useAnimation = 15;
	}

	// The Wilted Zone restoration system (GAME-03/Phase 6) is not implemented yet.
	// Use is disabled so the powder cannot be consumed with no effect; the dependency
	// is recorded as an inventory blocker until that system exists.
	public override bool CanUseItem(Player player) => false;
}
