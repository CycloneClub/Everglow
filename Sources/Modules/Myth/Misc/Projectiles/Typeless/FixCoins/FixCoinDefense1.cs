namespace Everglow.Myth.Misc.Projectiles.Typeless.FixCoins;

public class FixCoinDefense1 : FixCoinProjectile
{
	public override string HeatMapTexture()
	{
		return "heatmapGrey";
	}

	public override int PrefixID()
	{
		return 0;
	}

	public override int Level()
	{
		return 1;
	}
}
