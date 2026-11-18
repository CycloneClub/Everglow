using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Example.Projectiles;
using Everglow.Example.Pylon;

namespace Everglow.Example.Items;

public class ExamplePylon_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ExamplePylon>());
	}
}
