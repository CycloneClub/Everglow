using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class EucaLeafyGas : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 3;
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 2)
		{
			for (int g = 0; g < 60; g++)
			{
				Vector2 vel = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 15f).RotatedByRandom(MathHelper.TwoPi);
				var somg = new EucaLeafy_EmitGas
				{
					Velocity = vel,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(60, 75),
					Scale = Main.rand.NextFloat(50f, 155f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
		}
	}

	public void KillEffect()
	{

	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.ChaosState, 300);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return MathUtils.IntersectsCircleAABB(Projectile.Center, 100, targetHitbox);
	}
}
