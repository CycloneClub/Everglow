using Everglow.Example.Tiles;

namespace Everglow.Example.Items;

public class ExampleElevator_Item : ModItem
{
	public override string LocalizationCategory => Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ExampleElevator_Winch>());
		Item.width = 42;
		Item.height = 64;
		Item.value = 20000;
	}
}
