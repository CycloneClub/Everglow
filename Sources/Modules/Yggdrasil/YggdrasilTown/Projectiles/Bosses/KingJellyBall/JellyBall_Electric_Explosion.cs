using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;

public class JellyBall_Electric_Explosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 160;
		Projectile.height = 160;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		var wave = new JellyBallElectricExplosionWave
		{
			Velocity = Vector2.zeroVector,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(30, 40),
			Scale = Projectile.ai[0] / 5f,
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			ai = new float[] { 0.04f * MathF.Sqrt(Projectile.ai[0]) },
		};
		Ins.VFXManager.Add(wave);
		if (Projectile.ai[0] >= 10)
		{
			var wave2 = new JellyBallElectricExplosionWave
			{
				Velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(30, 40),
				Scale = Projectile.ai[0] / 12f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0.04f * MathF.Sqrt(Projectile.ai[0]) },
			};
			Ins.VFXManager.Add(wave2);
		}
		for (int i = 0; i < 2 * Projectile.ai[0]; i++)
		{
			var dustVFX = new ElectricCurrent
			{
				velocity = new Vector2(0, Main.rand.NextFloat(3, 4)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * MathF.Sqrt(Projectile.ai[0]) * 2,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(20, 60),
				scale = Main.rand.Next(1, 2) * Projectile.ai[0],
				ai = new float[] { Main.rand.NextFloat(1f), 0, Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		for (int i = 0; i < 10 * Projectile.ai[0]; i++)
		{
			var dustVFX = new JellyBallSparkElectricity
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(3, 4)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Projectile.ai[0],
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(40, 90),
				Scale = Main.rand.Next(1, 2) * Projectile.ai[0],
				ai = new float[] { Main.rand.NextFloat(1f, 8f), 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft <= 190)
		{
			Projectile.hostile = false;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float minDis = 170;
		if (Projectile.ai[0] >= 10)
		{
			minDis = 250;
		}
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < minDis;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < minDis;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < minDis;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < minDis;
		return bool0 || bool1 || bool2 || bool3;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(0.3f * (1 - timeValue), 1.6f * (1 - timeValue) * (1 - timeValue) * (1 - timeValue), 3f * (1 - timeValue) * (1 - timeValue), 0f);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 0.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 0.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 0.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 0.62f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radius / 2; h += 1)
		{
			c0.R = (byte)(h / radius * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 150f * Projectile.ai[0], 15 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.6f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.HitDirectionOverride = target.Center.X > Projectile.Center.X ? 1 : -1;
		base.ModifyHitNPC(target, ref modifiers);
	}
}
