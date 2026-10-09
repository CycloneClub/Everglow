namespace Everglow.CagedDomain.Tiles;

public class YellowDynastyShingles : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = true;
		Main.tileShine2[Type] = false;
		Main.ugBackTransition = 1000;
		DustType = 9;
		MinPick = 0;
		HitSound = SoundID.Grass;

		Main.tileSpelunker[Type] = true;
		AddMapEntry(new Color(229, 128, 4));
	}
}
