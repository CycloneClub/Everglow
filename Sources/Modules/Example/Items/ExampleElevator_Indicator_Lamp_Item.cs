using Everglow.Example.Tiles;

namespace Everglow.Example.Items;

public class ExampleElevator_Indicator_Lamp_Item : ModItem
{
	public override string LocalizationCategory => Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ExampleElevator_Indicator_Lamp>());
		Item.width = 20;
		Item.height = 36;
		Item.value = 1000;
	}
}
