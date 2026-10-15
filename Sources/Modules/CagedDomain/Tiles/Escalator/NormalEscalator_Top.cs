using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles.Escalator;

public class NormalEscalator_Top : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(153, 152, 151));
		HitSound = SoundID.DD2_SkeletonHurt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}
}
