namespace Everglow.Yggdrasil.KelpCurtain.Items.Pets;

/// <summary>
/// 熊猫宠物 - identity-only pet shell (D-18). The design row carries no
/// 伤害 / 价格 / 稀有度 / 效果 cells, so no pet projectile or pet buff is declared and no
/// gameplay behaviour is invented; the name selects the Pets category. Approved artwork
/// is absent from the repository.
/// </summary>
public class PandaPet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Pets;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
	}
}
