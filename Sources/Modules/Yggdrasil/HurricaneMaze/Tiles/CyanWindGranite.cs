namespace Everglow.Yggdrasil.HurricaneMaze.Tiles;

public class CyanWindGranite : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		DustType = DustID.Silver;
		AddMapEntry(new Color(65, 84, 63));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
	}
}
