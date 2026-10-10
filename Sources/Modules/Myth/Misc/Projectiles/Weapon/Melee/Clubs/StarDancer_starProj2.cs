using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class StarDancer_starProj2 : ModProjectile
{
	public override string Texture => "Everglow/" + ModAsset.StarDancer_Path;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 60;
		Projectile.hostile = false;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.width = 30;
		Projectile.height = 30;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft == 10)
		{
			Projectile.friendly = true;
			Vector2 v0 = new Vector2(Main.rand.NextFloat(1f, 1.2f), 0).RotatedByRandom(6.283);
			for (int i = 0; i < 5; i++)
			{
				Vector2 v1 = v0.RotatedBy(i / 2.5 * Math.PI);
				Vector2 v2 = v0.RotatedBy((i + 0.5) / 2.5 * Math.PI) * 3;
				Vector2 v3 = v0.RotatedBy((i + 1) / 2.5 * Math.PI);
				for (int j = 0; j < 15; j++)
				{
					Vector2 v4 = (v1 * j + v2 * (14 - j)) / 14f * Projectile.scale;
					Vector2 v5 = (v3 * j + v2 * (14 - j)) / 14f * Projectile.scale;
					Vector2 v6 = v2 * (14 - j) / 14f * Projectile.scale;
					var D = Dust.NewDustDirect(Projectile.Center + v4 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.5f);
					D.noGravity = true;
					D.velocity = v4;

					var D1 = Dust.NewDustDirect(Projectile.Center + v5 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.5f);
					D1.noGravity = true;
					D1.velocity = v5;

					var D2 = Dust.NewDustDirect(Projectile.Center + v6 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.3f);
					D2.noGravity = true;
					D2.velocity = v6;
				}
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}
