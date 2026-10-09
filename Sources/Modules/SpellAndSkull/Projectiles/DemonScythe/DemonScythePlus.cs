using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.SpellAndSkull.Dusts;
using Terraria.Audio;

namespace Everglow.SpellAndSkull.Projectiles.DemonScythe;

public class DemonScythePlus : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 60;
		Projectile.height = 60;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 16;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 3;
		Projectile.tileCollide = false;
	}

	public override void AI()
	{
		Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0.22f, 0f, 0.9f);
		if (timer < 30)
		{
			timer += 2;
		}

		if (Projectile.velocity.Length() < 48f)
		{
			Projectile.velocity *= 1.05f;
		}

		float vL = Projectile.velocity.Length() * 0.1f;
		vL = Math.Min(vL, 4f);
		float kSize = Math.Min(vL, 1f);
		for (float x = -vL; x < vL + 1; x += 2f)
		{
			float size = Main.rand.NextFloat(1.45f, 1.75f) * kSize;
			Vector2 lineVel = new Vector2(0, 24).RotatedBy(Math.PI * 0.5 - Main.timeForVisualEffects / 1.8 + x / 2d);
			lineVel = RotAndEclipse(lineVel);

			var d0 = Dust.NewDustDirect(Projectile.Center + lineVel - new Vector2(size * 4, size * 4.5f), 0, 0, ModContent.DustType<DemoFlame>(), 0, 0, 0, default, size);
			d0.fadeIn = 12f;
			Vector2 lineVel2 = new Vector2(0, 24).RotatedBy(Math.PI * 1 - Main.timeForVisualEffects / 1.8 + x / 2d);
			lineVel2 = RotAndEclipse(lineVel2);
			d0.velocity = Projectile.velocity + lineVel2 * 0.1f + Main.rand.NextVector2Unit() * 0.3f;
		}
		if (Collision.SolidCollision(Projectile.Center, 0, 0))
		{
			Projectile.Kill();
		}
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(Math.Min(0.8f, Projectile.velocity.Length() / 40f)), Projectile.Center);
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity * 2, Vector2.One, ModContent.ProjectileType<DemoHit>(), 0, 0, Projectile.owner, Projectile.velocity.Length() / 3f, Projectile.rotation + Main.rand.NextFloat(6.283f));
		float k = Math.Clamp(Projectile.velocity.Length() / 20f, 1f, 5f) / 1.3f;
		for (int x = 0; x < Projectile.velocity.Length() / 4 - 2; x++)
		{
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity * 2, Projectile.velocity.RotatedByRandom(6.283) * 0.4f, ModContent.ProjectileType<DemonScythePlusCrack>(), (int)(Projectile.damage * k * 0.1), (int)(Projectile.knockBack * k * 0.3), Projectile.owner, Projectile.velocity.Length() / 60f, Main.rand.NextFloat(8f, 24f));
			p.CritChance = (int)(Projectile.CritChance * k / 10);
			p.timeLeft = Main.rand.Next(45) + (int)Projectile.velocity.Length();
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		float k = Math.Clamp(Projectile.velocity.Length() / 30f, 1f, 5f) / 1.3f;
		for (int x = 0; x < Projectile.velocity.Length() / 12 - 2; x++)
		{
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity * 2, Projectile.velocity.RotatedByRandom(6.283) * 0.4f, ModContent.ProjectileType<DemonScythePlusCrack>(), (int)(Projectile.damage * k * 0.1), (int)(Projectile.knockBack * k * 0.3), Projectile.owner, Projectile.velocity.Length() / 120f, Main.rand.NextFloat(8f, 24f));
			p.timeLeft = Main.rand.Next(30) + (int)Projectile.velocity.Length();
		}
		modifiers.Knockback *= Projectile.velocity.Length() * 0.12f;
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity * 2, Vector2.One, ModContent.ProjectileType<DemoHit>(), 0, 0, Projectile.owner, Projectile.velocity.Length() / 3f, Projectile.rotation + Main.rand.NextFloat(6.283f));

		modifiers.FinalDamage *= k;
		target.AddBuff(BuffID.ShadowFlame, 60);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));
		DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));
		DrawMagicArray(Commons.ModAsset.Trail_5.Value, new Color(0.4f, 0.0f, 0.8f, 0));
		return false;
	}

	internal int timer = 0;

	public void DrawMagicArray(Texture2D tex, Color c0)
	{
		Texture2D Water = tex;
		var c1 = new Color(c0.R * 0.19f / 255f, c0.G * 0.19f / 255f, c0.B * 0.19f / 255f, c0.A * 0.19f / 255f);
		var c2 = new Color(c0.R * 0.09f / 255f, c0.G * 0.09f / 255f, c0.B * 0.09f / 255f, c0.A * 0.09f / 255f);
		float Size1 = (float)(Math.Sin((Main.timeForVisualEffects + 40) / 24) / 7d + 1);
		float Size2 = timer / 30f;
		DrawTexCircle(24, 25 * Size2, c0 * Size1, Projectile.Center - Main.screenPosition, Water, -Main.timeForVisualEffects / 7);
		DrawTexCircle(22, 12 * Size2, c1 * Size1, Projectile.Center - Main.screenPosition, Water, -Main.timeForVisualEffects / 27);
		DrawTexCircle(20, 12 * Size2, c2 * Size1, Projectile.Center - Main.screenPosition, Water, -Main.timeForVisualEffects / 127);
		DrawTexMoon(24, 25 * Size2, c0 * Size1, Projectile.Center - Main.screenPosition, ModAsset.BloomLight.Value, -Main.timeForVisualEffects / 1.8);
	}

	private void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + RotAndEclipse(new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot)), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + RotAndEclipse(new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4 + addRot)), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + RotAndEclipse(new Vector2(0, radius).RotatedBy(addRot)), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + RotAndEclipse(new Vector2(0, radius + width).RotatedBy(addRot)), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private void DrawTexMoon(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius * 5; h++)
		{
			Vector2 up = new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 0.27 + addRot);
			Vector2 down = new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 0.27 + addRot);
			up = RotAndEclipse(up);
			down = RotAndEclipse(down);
			circle.Add(new Vertex2D(center + up, color, new Vector3(h * 0.2f / radius, 1, 0)));
			circle.Add(new Vertex2D(center + down, color, new Vector3(h * 0.2f / radius, 0, 0)));
		}
		// circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		// circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
	private void DrawTexMoon(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius * 5; h++)
		{
			Vector2 up = new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 0.27 + addRot);
			Vector2 down = new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 0.27 + addRot);
			up = RotAndEclipse(up);
			down = RotAndEclipse(down);
			circle.Add(new Vertex2D(center + up, color, new Vector3(h * 0.2f / radius, 1, 0)));
			circle.Add(new Vertex2D(center + down, color, new Vector3(h * 0.2f / radius, 0, 0)));
		}
		// circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		// circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}
	private Vector2 RotAndEclipse(Vector2 orig)
	{
		return new Vector2(orig.X, orig.Y * 0.6f).RotatedBy(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X));
	}

	public static void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex)
	{
		float Wid = 6f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 91 + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 91d + 20.4) % 1f;

			if (Value1 < Value0)
			{
				float D0 = 1 - Value0;
				Vector2 Delta = EndPos - StartPos;
				vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				continue;
			}
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}
}
