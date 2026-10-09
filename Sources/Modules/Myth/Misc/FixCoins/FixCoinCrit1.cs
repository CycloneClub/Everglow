namespace Everglow.Myth.Misc.FixCoins;

public class FixCoinCrit1 : FixCoinItem
{
	public override int Level()
	{
		return 1;
	}

	public override void SSD()
	{
		Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.FixCoins.FixCoinCrit1>();
	}
}
