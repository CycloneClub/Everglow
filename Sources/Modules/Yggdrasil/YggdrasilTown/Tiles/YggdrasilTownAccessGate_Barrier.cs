namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilTownAccessGate_Barrier : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = false;
		AddMapEntry(Color.Black);
		MinPick = int.MaxValue;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 0;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
}
