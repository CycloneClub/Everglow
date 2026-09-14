namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 灵蛇玉卵 - consumable use item (使用类道具) awarded from the Valley of Lush and Moist.
/// The design row gives 10金 (10 gold) and 蓝色 (Blue rarity) and the effect
/// 在森雨幽谷顶部使用以召唤苍翠灵蛇 (use at the top of the Valley of Lush and Moist to summon
/// the Emerald Snake). The encounter itself is Phase 7 scope and the documented use
/// location needs a gate that the encounter's phase owns, so the item consumes without
/// summoning anything and both conditions are recorded as blockers.
/// </summary>
public class JadeSnakeEgg : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonItems;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.consumable = true;
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(gold: 10);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTime = Item.useAnimation = 12;
		Item.noMelee = true;
		Item.useTurn = true;
		Item.UseSound = SoundID.Roar;
		Item.useStyle = ItemUseStyleID.Swing;
	}
}
