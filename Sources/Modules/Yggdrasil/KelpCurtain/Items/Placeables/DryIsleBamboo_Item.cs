
using Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class DryIsleBamboo_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public int PlaceStyle = 0;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<IsleBamboo_H>());
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			PlaceStyle++;
			PlaceStyle %= 2;
			if (PlaceStyle == 1)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<IsleBamboo_V>());
			}
			else
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<IsleBamboo_H>());
			}
		}
		base.HoldItem(player);
	}
}
