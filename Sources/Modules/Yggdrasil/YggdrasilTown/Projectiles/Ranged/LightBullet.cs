using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

internal class LightBullet : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = ProjAIStyleID.Arrow;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;
		Projectile.alpha = 255;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 1;

		AIType = ProjectileID.Bullet;
	}

	public override void AI()
	{
		var somg = new LightFruitParticleDust
		{
			Velocity = Projectile.velocity * 0.3f,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(12, 15),
			Scale = Main.rand.NextFloat(12.20f, 32.35f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
		Lighting.AddLight(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 20, new Vector3(0.4f, 0.3f, 0.1f));
		base.AI();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Photolysis>(), 180);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texMain = Commons.ModAsset.StarSlash.Value;
		var drawColor = new Color(1f, 0.8f, 0f, 0f);
		float scale = 0.2f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, 0, texMain.Size() / 2f, scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, MathHelper.PiOver2, texMain.Size() / 2f, scale, SpriteEffects.None, 0);
		drawColor = new Color(0.6f, 0.4f, 0.2f, 0f);
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texMain.Size() / 2f, new Vector2(0.5f, 1f), SpriteEffects.None, 0);

		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, -MathHelper.PiOver4, texMain.Size() / 2f, new Vector2(0.3f, scale * 0.7f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, MathHelper.PiOver4, texMain.Size() / 2f, new Vector2(0.3f, scale * 0.7f), SpriteEffects.None, 0);

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<LightStartEffect_bullet>(), 0, 0, Projectile.owner);

		var lightStartEffectProjectiles = new List<Projectile>();

		// Collect all active LightStartEffect_bullet projectiles
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active && proj.type == ModContent.ProjectileType<LightStartEffect_bullet>())
			{
				lightStartEffectProjectiles.Add(proj);
			}
		}

		// Sort the collected projectiles by timeLeft in descending order
		lightStartEffectProjectiles.Sort((a, b) => b.timeLeft.CompareTo(a.timeLeft));

		// Get the top 5 projectiles (if they exist)
		var topFiveProjectiles = new HashSet<Projectile>();
		for (int i = 0; i < Math.Min(8, lightStartEffectProjectiles.Count); i++)
		{
			topFiveProjectiles.Add(lightStartEffectProjectiles[i]);
		}

		// Set timeLeft to 150 for other projectiles with timeLeft > 150
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active && proj.type == ModContent.ProjectileType<LightStartEffect_bullet>() && proj.timeLeft > 150 && !topFiveProjectiles.Contains(proj))
			{
				proj.timeLeft = 150;
			}
		}
	}
}
