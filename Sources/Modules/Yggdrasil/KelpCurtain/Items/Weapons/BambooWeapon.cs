namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

/// <summary>
/// 竹制武器 - identity-only melee weapon shell (D-18). The design row carries no
/// 伤害 / 价格 / 稀有度 / 效果 cells, so no gameplay behaviour can be transcribed and none is
/// invented; the name selects the MeleeWeapons category. Approved artwork is absent from
/// the repository.
/// </summary>
public class BambooWeapon : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = 1;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
	}
}
