using Everglow.Yggdrasil.KelpCurtain.Tiles;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class DevilHeartIronOre_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<DevilHeartIronOre>());
	}
}
