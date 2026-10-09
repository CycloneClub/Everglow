using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_Arrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 1;
		Projectile.timeLeft = 2400;
		Projectile.alpha = 0;
		Projectile.penetrate = 30;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	private void AddLight()
	{
		if (TimeToKill < 0)
		{
			Lighting.AddLight(Projectile.Center, 0f, 0.4f, 0.8f);
		}
	}

	public override void AI()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (TimeToKill >= 0 && TimeToKill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeToKill > 0)
		{
			Projectile.velocity *= 0.01f;
		}

		TimeToKill--;
		if (TimeToKill < 0)
		{
		}
		AddLight();

		if (Main.rand.NextBool(8) && TimeToKill <= 0)
		{
			Vector2 newVelocity = Projectile.velocity;
			var smog = new Fevens_ArrowTrail
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,

				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(17, 25),
				Scale = Main.rand.NextFloat(1f, 2f),
				Rotation = newVelocity.ToRotation(),

				ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public int TimeToKill = -1;

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		HitToAnything();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		HitToAnything();
		Projectile.tileCollide = false;
		return false;
	}

	private void HitToAnything()
	{
		Projectile.velocity = Projectile.oldVelocity;
		if (TimeToKill < 0)
		{
			for (int t = 0; t < 16; t++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 6f)).RotatedByRandom(MathHelper.TwoPi);
				var smog = new Fevens_ArrowTrail
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,

					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(27, 35),
					Scale = Main.rand.NextFloat(1f, 2f),
					Rotation = newVelocity.ToRotation(),

					ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
				};
				Ins.VFXManager.Add(smog);
			}
		}
		TimeToKill = 90;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D star = ModAsset.Fevens_Arrow.Value;
		if (TimeToKill < 0)
		{
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(250, 250, 250, 150), Projectile.rotation + MathHelper.PiOver4, star.Size() / 2f, 0.75f, SpriteEffects.None, 0);
		}
		return false;
	}
}
