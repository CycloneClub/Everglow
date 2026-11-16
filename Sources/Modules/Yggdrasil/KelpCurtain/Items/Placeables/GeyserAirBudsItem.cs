using Everglow.Yggdrasil.KelpCurtain.Tiles.GeyserAirBuds;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class GeyserAirBudsItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<GeyserAirBuds_PlacePreview>());
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.width = 16;
		Item.height = 16;
	}

	public override bool CanUseItem(Player player)
	{
		Item.createTile = ModContent.TileType<GeyserAirBuds>();
		return base.CanUseItem(player);
	}

	public override bool? UseItem(Player player) => base.UseItem(player);

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft)
		{
			Item.createTile = ModContent.TileType<GeyserAirBuds>();
			Main.placementPreview = false;
		}
		else
		{
			Item.createTile = ModContent.TileType<GeyserAirBuds_PlacePreview>();
		}
		Main.placementPreview = true;
	}
}
