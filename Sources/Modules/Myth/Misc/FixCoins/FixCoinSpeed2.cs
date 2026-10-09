namespace Everglow.Myth.Misc.FixCoins;

public class FixCoinSpeed2 : FixCoinItem
{
	public override int Level()
	{
		return 2;
	}

	public override void SSD()
	{
		Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.FixCoins.FixCoinSpeed2>();
	}
}
