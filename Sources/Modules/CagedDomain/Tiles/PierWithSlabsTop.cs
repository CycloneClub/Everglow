using Everglow.Commons.TileHelper;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class PierWithSlabsTop : ShapeDataTile
{
	public override void SetStaticDefaults()
	{
		TotalWidth = 13;
		TotalHeight = 8;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = false;
		DustType = DustID.Stone;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 8;
		TileObjectData.newTile.Width = 13;
		TileObjectData.newTile.CoordinateHeights = new int[8];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.Origin = new Point16(6, 0);
		TileObjectData.addTile(Type);

		// Etc
		AddMapEntry(new Color(86, 86, 86));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	//public override bool CanExplode(int i, int j)
	//{
	//	return false;
	//}

	//public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	//{
	//	return false;
	//}
}
