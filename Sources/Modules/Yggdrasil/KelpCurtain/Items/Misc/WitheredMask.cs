namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

public class WitheredMask : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Vanity;

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
