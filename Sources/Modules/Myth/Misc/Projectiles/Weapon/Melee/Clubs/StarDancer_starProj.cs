using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class StarDancer_starProj : ModProjectile
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
			for (int i = 0; i < 3f; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.YellowStarDust, 0, 0, 0, default, 1f);
				dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f);
			}
			ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings()
			{
				PositionInWorld = Projectile.Center,
			});
			ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.StellarTune, new ParticleOrchestraSettings()
			{
				PositionInWorld = Projectile.Center,
			});
			Projectile.friendly = true;
		}
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
