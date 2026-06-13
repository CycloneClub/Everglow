using Terraria.Audio;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternFlameBullet : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.timeLeft = 3600;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = false;
		Projectile.penetrate = 1;
		Projectile.aiStyle = -1;
		Projectile.extraUpdates = 4;
	}

	public override void AI()
	{
		if (Projectile.timeLeft < 3597)
		{
			Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity * 0.3f);
			dust.scale = 1.2f;
			dust.noGravity = true;
		}
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.6f, 0.6f));
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		target.AddBuff(BuffID.OnFire3, 300);
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
		switch (Main.rand.Next(3))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_hit_1_Mod), Projectile.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_hit_2_Mod), Projectile.Center);
				break;
			case 2:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_hit_3_Mod), Projectile.Center);
				break;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<Lantern_ExplosionEffect>(), Projectile.damage, 2, Projectile.owner, 3);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D black = ModAsset.LanternBullet_black.Value;
		Texture2D bullet = ModContent.Request<Texture2D>(Texture).Value;

		Main.EntitySpriteDraw(black, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, black.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bullet, Projectile.Center - Main.screenPosition, null, Color.White * 0.75f, Projectile.rotation, bullet.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}
}
