using Everglow.Commons.TileHelper;
using Everglow.Example.Items;
using Terraria.Localization;

namespace Everglow.Example.Walls;

public class ExampleMirrorWall : MirrorWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.Glass;
		AddMapEntry(new Color(160, 185, 205), Language.GetText(ExampleMirrorItem.TextKey + "Name"));
	}
}
