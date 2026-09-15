using Everglow.Yggdrasil.KelpCurtain.Tiles.ForestRainVines;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class ForestRainVineTile_Thin_Item : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.value = 40;
		Item.DefaultToPlaceableTile(ModContent.TileType<ForestRainVineTile_Thin>());
	}

	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}
}
