using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class PearShapedNeedle_Needle : ModProjectile
{
	public float Timer = 0;

	public Vector2 StartPosition = default;

	public Vector2 EndPosition = default;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.width = 12;
		Projectile.height = 12;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = 1;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1;
		Projectile.DamageType = DamageClass.Ranged;
	}

	public override void OnSpawn(IEntitySource source)
	{
		StartPosition = Projectile.Center;
		Vector2 endPos = Projectile.position;
		Vector2 vel = Projectile.velocity.NormalizeSafe() * 5;
		bool hit = false;
		for (int i = 0; i < 30; i++)
		{
			endPos += vel;
			if (Collision.SolidCollision(endPos, Projectile.width, Projectile.height))
			{
				hit = true;
			}
			Rectangle hitbox = new Rectangle((int)endPos.X, (int)endPos.Y, Projectile.width, Projectile.height);
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && npc.Hitbox.Intersects(hitbox))
				{
					hit = true;
				}
			}
			if (hit)
			{
				for (int g = 0; g < 6; g++)
				{
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
					var spark = new FireSpark_MetalStabDust
					{
						velocity = newVelocity,
						Active = true,
						Visible = true,
						position = endPos,
						maxTime = Main.rand.Next(1, 25),
						scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(10f, 27.0f)),
						rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
					};
					Ins.VFXManager.Add(spark);
				}
				break;
			}
		}
		Projectile.position = endPos;
		EndPosition = Projectile.Center;
		for (int k = 0; k < 5; k++)
		{
			var dust = new PearShapedNeedle_Dust
			{
				Velocity = vel * (3 + k * 0.5f),
				Active = true,
				Visible = true,
				Position = EndPosition,
				MaxTime = Main.rand.NextFloat(25f, 35f),
				Scale = Main.rand.NextFloat(1f, 2f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RanSeed = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(dust);
		}

		var trail = new PearShapedNeedle_Trail
		{
			Active = true,
			Visible = true,
			Position_Start = StartPosition,
			Position_End = EndPosition,
			MaxTime = Main.rand.NextFloat(25f, 35f),
		};
		Ins.VFXManager.Add(trail);
		if (hit)
		{
			var dust = new PearShapedNeedle_HitStar
			{
				Active = true,
				Visible = true,
				Position = EndPosition,
				MaxTime = Main.rand.NextFloat(25f, 35f),
				Rotation = vel.ToRotation(),
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override void AI() => base.AI();

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
