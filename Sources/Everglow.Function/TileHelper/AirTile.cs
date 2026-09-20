
namespace Everglow.Commons.TileHelper;

public class AirTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = false;
		AddMapEntry(Color.Transparent);
	}
}
