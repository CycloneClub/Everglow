using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.MEAC.PlanetBeFall.Buffs;
using Everglow.MEAC.PlanetBeFall.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.MEAC.PlanetBeFall.Projectiles.NonIIIDProj.PlanetBefallExplosion;

public class PlanetBefallExplosion : ModProjectile// , IWarpProjectile
{
	public float BlurOffset = 0;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 6;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(new SoundStyle(ModAsset.Crystal_Burst_Strong_Mod), Projectile.Center);
		GenerateSmog((int)(0.7f * Projectile.ai[0]));
		GenerateFire((int)(0.75f * Projectile.ai[0]));
		GenerateSmog((int)(0.3f * Projectile.ai[0]));
	}

	public void GenerateSmog(int Frequency)
	{
		float mulVelocity = Projectile.ai[0] / 5f;
		for (int g = 0; g < Frequency * 0.6; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(45f, 125f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogLine_front
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + newVelocity * Main.rand.NextFloat(-0.5f, 4f),
				MaxTime = Main.rand.Next(45, 68),
				Scale = Main.rand.NextFloat(60f, 220f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 4,
				MaxTime = Main.rand.Next(60, 90),
				Scale = Main.rand.NextFloat(2f, 7f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency * 2; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * MathF.Sqrt(Main.rand.NextFloat(0f, 1f)) * 2).RotatedByRandom(MathHelper.TwoPi);
			var somg = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(MathF.Sqrt(Main.rand.NextFloat(0f, 1f)), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(60, 90),
				Scale = Main.rand.NextFloat(2f, 7f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateFire(int Frequency)
	{
		float mulVelocity = Projectile.ai[0] / 4f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new VFXs.PlanetBeFallFireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(0, 15), Main.rand.NextFloat(-15, 15)).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(45, 75),
				Scale = Main.rand.NextFloat(2f, 7f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
		for (int g = 0; g < Frequency * 2; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * MathF.Sqrt(Main.rand.NextFloat(0f, 1f)) * 2).RotatedByRandom(MathHelper.TwoPi);
			var fire = new VFXs.PlanetBeFallFireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(MathF.Sqrt(Main.rand.NextFloat(0f, 1f)), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(45, 75),
				Scale = Main.rand.NextFloat(0.5f, 2f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
		for (int g = 0; g < Frequency * 3; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 28f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new Spark_RockCrackDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + newVelocity * 4,
				MaxTime = Main.rand.Next(160, 250),
				Scale = Main.rand.NextFloat(2f, 7f) * Projectile.ai[0],
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency * 2; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new Spark_RockCrackDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + newVelocity * 2,
				MaxTime = Main.rand.Next(160, 250),
				Scale = Main.rand.NextFloat(0.5f, 1f) * Projectile.ai[0],
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void AI()
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		Projectile.velocity *= 0;
		if (Projectile.timeLeft <= 199)
		{
			Projectile.friendly = false;
		}

		BlurOffset = 0.5f * MathF.Max(MathF.Sin(timeValue * MathF.PI * 1.5f), 0);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		return bool0 || bool1 || bool2 || bool3;
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D shadow = Commons.ModAsset.Point.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50, 0);
		var c = new Color(1f * MathF.Sqrt(1 - timeValue), 1f * (1 - timeValue) * (1 - timeValue), 0.1f * (1 - timeValue), 0f);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, c * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
		var cDark = new Color(0, 0, 0, 1f - timeValue);

		// DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 20 * (1 - timeValue) * Projectile.ai[0], cDark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value);
		// DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], c * 0.4f , Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D shadow = Commons.ModAsset.Point_black.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 25, 0);
		var c = new Color(1f * MathF.Sqrt(1 - timeValue), 1f * (1 - timeValue) * (1 - timeValue), 0.1f * (1 - timeValue), 0f);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] * 0.2f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.Textures_Star.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark * dark) * Projectile.ai[0] * 0.1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 0.1f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
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
		float colorV = 0.5f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 11f * Projectile.ai[0], 12 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.6f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// For code safty, you can't prtrificate a boss.
		if (!target.boss || target.type == NPCID.EyeofCthulhu || target.type == NPCID.BrainofCthulhu || target.type == NPCID.Skeleton || target.type == NPCID.SkeletronPrime || target.type == NPCID.Deerclops || target.type == NPCID.KingSlime || target.type == NPCID.QueenBee || target.type == NPCID.QueenBee || target.type == NPCID.DukeFishron || target.type == NPCID.Spazmatism || target.type == NPCID.Retinazer || target.type == NPCID.Plantera || target.type == NPCID.CultistBoss)
		{
			target.AddBuff(ModContent.BuffType<Petrification>(), 240);
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (target.Center.X < Projectile.Center.X)
		{
			modifiers.HitDirectionOverride = -1;
		}
		else
		{
			modifiers.HitDirectionOverride = 1;
		}
		base.ModifyHitNPC(target, ref modifiers);
	}
}
