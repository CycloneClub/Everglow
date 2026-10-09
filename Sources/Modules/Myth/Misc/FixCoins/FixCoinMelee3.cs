namespace Everglow.Myth.Misc.FixCoins;

public class FixCoinMelee3 : FixCoinItem
{
	public override int Level()
	{
		return 3;
	}

	public override void SSD()
	{
		Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.FixCoins.FixCoinMelee3>();
	}
}
