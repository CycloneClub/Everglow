using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class LightArrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;

		Projectile.arrow = true;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Ranged;

		Projectile.penetrate = 1;

		Projectile.timeLeft = 1200;
	}

	public void GenerateParticles(int duplicateTimes = 1)
	{
		float mulMaxTime = 1f;
		if (Projectile.timeLeft > 1100)
		{
			mulMaxTime = 1f - (Projectile.timeLeft - 1100) / 100f;
		}
		if (Projectile.timeLeft < 150)
		{
			mulMaxTime = (Projectile.timeLeft - 90f) / 60f;
		}
		if (mulMaxTime < 0)
		{
			return;
		}
		for (int i = 0; i < duplicateTimes; i++)
		{
			Vector2 newVelocity = new Vector2(0, 1.2f).RotatedBy(Main.time * 0.15f + Projectile.whoAmI + (float)i / duplicateTimes * MathHelper.TwoPi);
			var somg = new LightFruitParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity,
				MaxTime = Main.rand.Next(37, 145) * mulMaxTime,
				Scale = Main.rand.NextFloat(12.20f, 32.35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(1.0f, 6.3f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int i = 0; i < duplicateTimes; i++)
		{
			Vector2 newVelocity = new Vector2(0, 1.2f).RotatedBy(-Main.time * 0.12f + Projectile.whoAmI + (float)i / duplicateTimes * MathHelper.TwoPi);
			var somg = new LightFruitParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity,
				MaxTime = Main.rand.Next(37, 145) * mulMaxTime,
				Scale = Main.rand.NextFloat(12.20f, 32.35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(1.0f, 6.3f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void AI()
	{
		GenerateParticles(3);

		// Apply gravity after a quarter of a second
		Projectile.ai[0] += 1f;
		if (Projectile.ai[0] >= 15f)
		{
			Projectile.ai[0] = 15f;
			Projectile.velocity.Y += 0.1f;
		}

		// The projectile is rotated to face the direction of travel
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		// Cap downward velocity
		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
		Lighting.AddLight(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 20, new Vector3(0.8f, 0.6f, 0));
		if (Main.rand.NextBool(5))
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LampWood_Dust_fluorescent_appear>());
			dust.alpha = 0;
			dust.rotation = Main.rand.NextFloat(0.3f, 0.7f);
			dust.scale *= 2f;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathF.PI, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		var texGlow = ModAsset.LightArrow_glow.Value;
		Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation + MathF.PI, texGlow.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Photolysis>(), 180);
	}

	public override void OnKill(int timeLeft)
	{
		// Basic OnKill Instructions
		// -----------------------
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
		for (int i = 0; i < 20; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LampWood_Dust_fluorescent_appear>());
			dust.alpha = 0;
			dust.rotation = Main.rand.NextFloat(0.3f, 0.7f);
			dust.scale *= 2f;
		}

		// Generate Post-destruction VFX
		// -----------------------------
		// Create new LightStartEffect_arrow projectile at the locaiton where this projectile is killed
		Projectile.NewProjectileDirect(
			Projectile.GetSource_FromAI(),
			Projectile.Center,
			Vector2.Zero,
			ModContent.ProjectileType<LightStartEffect_arrow>(),
			0,
			0,
			Projectile.owner);

		// Collect all active LightStartEffect_arrow projectiles
		// Sort the collected projectiles by timeLeft in descending order
		// Get the top 5 projectiles (if they exist)
		var topFiveActiveProjectiles = Main.projectile
			.Where(proj => proj.active && proj.type == ModContent.ProjectileType<LightStartEffect_arrow>())
			.OrderByDescending(l => l.timeLeft)
			.Take(5)
			.ToHashSet();

		// Set timeLeft to 150 for other projectiles with timeLeft more than 150
		foreach (var item in Main.projectile
			.Where(proj =>
				proj.active &&
				proj.type == ModContent.ProjectileType<LightStartEffect_arrow>() &&
				proj.timeLeft > 150 &&
				!topFiveActiveProjectiles.Contains(proj)))
		{
			item.timeLeft = 150;
		}
	}
}
