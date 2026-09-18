using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class ClothesShopSofa_Feature_Nature : ModTile
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
		AddMapEntry(new Color(127, 89, 77));
		HitSound = SoundID.DD2_SkeletonHurt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}
}
