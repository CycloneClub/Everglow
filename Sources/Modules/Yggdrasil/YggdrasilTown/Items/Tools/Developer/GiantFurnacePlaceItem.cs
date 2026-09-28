using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;
using Everglow.Yggdrasil.YggdrasilTown.Walls;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class GiantFurnacePlaceItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<TraineeEquipmentsShelf>());
	}

	public override void HoldItem(Player player)
	{
		// Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		TileUtils.PlaceFrameImportantTilesAtTileObjectDataOrigin(Main.MouseWorld.ToTileCoordinates(),ModContent.TileType<TraineeEquipmentsShelf>());
		return true;
	}

	public override bool? UseItem(Player player)
	{
		return true;
	}
}
