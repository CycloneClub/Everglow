namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;

public class CyanVineStone : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<Dusts.CyanVine>();
		MineResist = 4f;
		Main.tileSpelunker[Type] = true;
		AddMapEntry(new Color(84, 130, 154));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
