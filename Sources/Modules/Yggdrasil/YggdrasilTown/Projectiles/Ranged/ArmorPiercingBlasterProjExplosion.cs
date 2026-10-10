using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class ArmorPiercingBlasterProjExplosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 60;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 2;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.width = 220;
		Projectile.height = 220;
		Timer = 0;
	}

	public int Timer = 0;

	public override void OnSpawn(IEntitySource source)
	{
		for (int step = 0; step < 100; step++)
		{
			var dustVFX = new ArmorPiercingSpark
			{
				Velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 4).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -Main.rand.NextFloat(11)),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 20).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = 200,
				Scale = Main.rand.NextFloat(12, 30),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		for (int step = 0; step < 100; step++)
		{
			var dustVFX = new ArmorPiercingSpark
			{
				Velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 6).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -Main.rand.NextFloat(11)),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 20).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = 200,
				Scale = Main.rand.NextFloat(4, 15),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		for (int i = 0; i < 30; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(2.6f, 8.4f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -Main.rand.NextFloat(18));
			var dust = new ArmorPiercingTrailDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(20, 60),
				Scale = Main.rand.NextFloat(1f, 2f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override void AI()
	{
		Timer++;
		if (Timer > 5)
		{
			Projectile.friendly = false;
		}
		else
		{
			for (int step = 0; step < 10; step++)
			{
				var dustVFX = new ArmorPiercingSpark
				{
					Velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 4).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -Main.rand.NextFloat(2)),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 20).RotatedByRandom(MathHelper.TwoPi),
					MaxTime = 200,
					Scale = Main.rand.NextFloat(4, 30),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					ai = new float[] { 0, 0, 0 },
				};
				Ins.VFXManager.Add(dustVFX);
			}
		}
	}

	public void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radius / 2; h += 1)
		{
			c0.R = (byte)(h / radius * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(0, 0, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = Timer;
		float colorV = 0.5f;

		Texture2D t = Commons.ModAsset.Trail.Value;
		float width = Projectile.timeLeft;

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 20, width * 2, new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Armor15Cracked>(), 600);
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void OnKill(int timeLeft) => base.OnKill(timeLeft);

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		return false;
	}
}
