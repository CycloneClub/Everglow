using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class FevensPortrait : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileUtils.DefaultToMultiTileWall(3, 5);
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		AddMapEntry(new Color(215, 212, 216));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}
}
