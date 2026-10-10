using Everglow.Commons.Templates;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;

public class Duskfin : FishBase
{
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		base.SetDefaults();
		Item.width = 32;
		Item.height = 20;
		Item.scale = 0.1f;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 1, silver: 23, copper: 36);
	}
}
