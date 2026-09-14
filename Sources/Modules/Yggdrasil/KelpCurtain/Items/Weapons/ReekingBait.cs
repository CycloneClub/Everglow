namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class ReekingBait : ModItem
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

		Item.value = Item.buyPrice(silver: 20);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;

		Item.useTime = Item.useAnimation = 12;
		Item.noMelee = true;
		Item.useTurn = true;
		Item.UseSound = SoundID.Roar;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	// Encounter blocker (Phase 7): the design summons 巨翼龙, a Phase 7 Giant Winged Dragon
	// encounter. CanUseItem is gated off and no NPC type is referenced, so the item is not
	// consumed while it would spawn nothing; revert the gate when that encounter exists.
	//
	// Recipe blocker: the design 合成方式 cell lists 血云母 + 干枯心脏, both Phase 7 Giant Winged
	// Dragon items absent from the repository; referencing an absent type is a compile error,
	// so no AddRecipes body is written.
	public override bool CanUseItem(Player player)
	{
		return false;
	}
}
