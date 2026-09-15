using Everglow.CagedDomain.Tiles.Escalator;
using Terraria.GameInput;

namespace Everglow.CagedDomain.Items;

public class NormalEscalator_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public int PlaceState = 0;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<NormalEscalator>());
		Item.width = 40;
		Item.height = 34;
		Item.value = 40000;
	}

	public override void HoldItem(Player player)
	{
		if (PlayerInput.Triggers.JustReleased.MouseRight)
		{
			PlaceState = (PlaceState + 1) % 2;
		}
		switch (PlaceState)
		{
			case 0:
				Item.DefaultToPlaceableTile(ModContent.TileType<NormalEscalator>());
				Item.width = 40;
				Item.height = 34;
				Item.value = 40000;
				break;
			case 1:
				Item.DefaultToPlaceableTile(ModContent.TileType<NormalEscalator_Top>());
				Item.width = 40;
				Item.height = 34;
				Item.value = 40000;
				break;
		}
		Item.placeStyle = Math.Max(-player.direction, 0);
	}
}
