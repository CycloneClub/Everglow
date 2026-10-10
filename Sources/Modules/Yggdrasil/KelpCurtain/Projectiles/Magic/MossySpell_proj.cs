using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class MossySpell_proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.scale = 0.85f;
	}

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
		if (Projectile.velocity.Y < 12)
		{
			Projectile.velocity.Y += 0.15f;
		}
		else
		{
			Projectile.velocity.Y = 12f;
		}
		float colTime = CollisionPredict();
		if (colTime < 0)
		{
			colTime = 10;
		}

		var dustVFX = new MossBlossomDustFace
		{
			Velocity = Projectile.velocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(0, 3).RotatedByRandom(Math.PI * 2),
			MaxTime = Main.rand.Next(30, 46) * colTime / 10f,
			Scale = Main.rand.NextFloat(12, 16),
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			ai = new float[] { 0, 0, 0 },
		};
		Ins.VFXManager.Add(dustVFX);
		if (Main.rand.NextBool(4))
		{
			var dustVFX2 = new MossBlossomDustSide
			{
				Velocity = Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(8, 12),
				MaxTime = Main.rand.Next(25, 30) * colTime / 10f,
				Scale = Main.rand.NextFloat(12, 16),
				Rotation = Projectile.velocity.ToRotationSafe() - MathHelper.PiOver4 * 3 + MathHelper.PiOver2,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX2);
		}

		if (Main.rand.NextBool(4))
		{
			var dustVFX3 = new MossBlossomDustSide
			{
				Velocity = Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity.NormalizeSafe().RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(8, 12),
				MaxTime = Main.rand.Next(25, 30) * colTime / 10f,
				Scale = Main.rand.NextFloat(12, 16),
				Rotation = Projectile.velocity.ToRotationSafe() - MathHelper.PiOver4 * 3 - MathHelper.PiOver2,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX3);
		}
	}

	public int CollisionPredict()
	{
		Vector2 posCheck = Projectile.position;
		Vector2 velCheck = Projectile.velocity;
		for (int i = 0; i < 10; i++)
		{
			posCheck += velCheck;
			velCheck.Y += 0.15f;
			if (Collision.SolidCollision(posCheck, Projectile.width, Projectile.height))
			{
				return i;
			}
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && !npc.dontTakeDamage)
				{
					if (Rectangle.Intersect(new Rectangle((int)posCheck.X, (int)posCheck.Y, Projectile.width, Projectile.height), npc.Hitbox) != Rectangle.Empty)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D tex = ModAsset.MossySpell_proj.Value;
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int t = 0; t < 25; t++)
		{
			Vector2 phi = new Vector2(0, Main.rand.Next(14, 22)).RotatedBy(t / 25f * MathHelper.TwoPi);
			var dustVFX4 = new MossBlossomDustSide
			{
				Velocity = phi.RotatedBy(0.7f) * 0.2f,
				Active = true,
				Visible = true,
				Position = Projectile.Center + phi * 0.4f,
				MaxTime = Main.rand.Next(15, 40),
				Scale = Main.rand.NextFloat(12, 16),
				Rotation = phi.RotatedBy(0.7f).ToRotationSafe() - MathHelper.PiOver4 * 3,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX4);
		}
		base.OnKill(timeLeft);
	}
}
