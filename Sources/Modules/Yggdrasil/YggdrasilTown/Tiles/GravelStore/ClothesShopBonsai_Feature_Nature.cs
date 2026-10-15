using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class ClothesShopBonsai_Feature_Nature : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(66, 99, 50));
		HitSound = SoundID.DD2_SkeletonHurt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}
}
