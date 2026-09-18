using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class BakeryCounter : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileUtils.DefaultToMultiTileAnchorBottom(7, 6);
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(76, 57, 46));
		HitSound = SoundID.DD2_SkeletonHurt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}
}
