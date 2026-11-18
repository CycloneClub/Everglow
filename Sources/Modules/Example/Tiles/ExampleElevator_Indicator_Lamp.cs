using Everglow.Commons.Templates.Furniture.Elevator;

namespace Everglow.Example.Tiles;

public class ExampleElevator_Indicator_Lamp : FloorIndicatorTileBase
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetCustomDefaults() => base.SetCustomDefaults();

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY is >= 54 and <= 72)
		{
			Lighting.AddLight(new Vector2(i * 16, j * 16), new Vector3(1f, 0.8f, 0.3f));
		}
	}
}
