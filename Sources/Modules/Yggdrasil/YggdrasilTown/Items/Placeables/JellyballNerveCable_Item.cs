using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class JellyballNerveCable_Item : CableTileItem
{
	public override int TileType => ModContent.TileType<JellyBallNerveCable>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Item.width = 24;
		Item.height = 24;
	}
}
