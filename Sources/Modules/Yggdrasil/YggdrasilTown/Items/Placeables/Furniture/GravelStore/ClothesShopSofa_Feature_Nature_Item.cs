using Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;
using Terraria.GameInput;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.GravelStore;

public class ClothesShopSofa_Feature_Nature_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public int PlaceState = 0;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ClothesShopSofa_Feature_Nature>());
		Item.width = 22;
		Item.height = 22;
	}

	public override void HoldItem(Player player)
	{
		if (PlayerInput.Triggers.JustReleased.MouseRight)
		{
			PlaceState = (PlaceState + 1) % 2;
		}
		Item.placeStyle = Math.Max(player.direction, 0) + PlaceState * 2;
		base.HoldItem(player);
	}
}
