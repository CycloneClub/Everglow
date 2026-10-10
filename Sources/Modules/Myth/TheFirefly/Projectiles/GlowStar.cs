using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Audio;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class GlowStar : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1000;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1f;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
	}

	public int targetWhoAmI = -1;

	public override void AI()
	{
		Projectile.velocity *= 0.9993f;
		float minDistance = 450 - Projectile.ai[0] * 25;

		if (targetWhoAmI == -1)
		{
			int tryRandomTarget = Main.rand.Next(Main.npc.Length);
			NPC target = Main.npc[tryRandomTarget];
			Vector2 toTarget = target.Center - Projectile.Center - Projectile.velocity;
			float Dis = toTarget.Length();

			if (Dis < minDistance)
			{
				if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy() && target.active)
				{
					targetWhoAmI = target.whoAmI;
				}
			}
		}
		else
		{
			if (!Main.npc[targetWhoAmI].active)
			{
				targetWhoAmI = -1;
				return;
			}
			Vector2 toTarget = Main.npc[targetWhoAmI].Center - Projectile.Center - Projectile.velocity;
			Projectile.velocity = Projectile.velocity * 0.96f + Vector2.Normalize(toTarget) * 3f * 0.04f;
			if (Projectile.velocity.Length() > 15f)
			{
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15f;
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float k1 = 200;
		float k0 = (1000 - Projectile.timeLeft) / k1;
		float k2 = 1f;
		float mulFactor = 1f;
		if (Projectile.timeLeft <= 1000 - k1)
		{
			k0 = 1;
		}

		if (Projectile.timeLeft < 200)
		{
			k2 = Projectile.timeLeft / 200f;
		}

		var c0 = new Color((1f - k0) * 0.6f, 1.5f - k0, 2f - k0, 0);

		var bars = new List<Vertex2D>();
		float width = 24;
		float k3 = Projectile.ai[1] / 60f;
		if (Projectile.ai[1] > 0)
		{
			width *= k3;
		}

		width *= (k0 / 1.8f + 0.2f) / (Projectile.ai[0] + 3) * 3.5f * k2;
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}
		Color c1 = new Color(0.2f, 0.4f, 0.6f, 0);
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)TrueL;

			float x0 = factor * mulFactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f) - Main.screenPosition, c1, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f) - Main.screenPosition, c1, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * mulFactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c1, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c1, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c1, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c1, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_2.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Texture2D dark = ModAsset.BlueFlameDark.Value;
		Texture2D Light = ModAsset.GlowStar.Value;
		float scale = (k0 / 1.8f + 0.2f) / (Projectile.ai[0] + 3) * k2;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, Color.White, Projectile.rotation, dark.Size() / 2f, scale * 1.8f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, c0, Projectile.rotation, Light.Size() / 2f, scale * 3.5f, SpriteEffects.None, 0);
		// Color c2 = new Color(0, 1.5f - k0, 2f - k0, 0);
		// Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, c2 * (1.1f - k0), Projectile.rotation, Light.Size() / 2f, new Vector2(16f, scale * 1.5f), SpriteEffects.None, 0);
		// Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, c2 * (1.1f - k0), Projectile.rotation, Light.Size() / 2f, new Vector2(scale * 1.5f, 4f), SpriteEffects.None, 0);
		return false;
	}

	public override bool PreKill(int timeLeft)
	{
		return timeLeft < 995;
	}

	public override void OnKill(int timeLeft)
	{
		float value = Math.Min(Projectile.damage / 30f, 1f);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowStarExplosion>(), 0, 0, Projectile.owner, 2.2f / (Projectile.ai[0] + 2) * 0.6f * value, 0.3f);
		SoundEngine.PlaySound(SoundID.Item38, Projectile.Center);
		float k1 = Math.Clamp(Projectile.velocity.Length(), 1, 3);
		float k2 = Math.Clamp(Projectile.velocity.Length(), 6, 10);
		float k0 = 1f / (Projectile.ai[0] + 2) * 2 * k2;
		for (int j = 0; j < 30; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(1, 3f), 0).RotatedByRandom(6.283) * Projectile.scale * k1;
			int dust0 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<BlueGlowAppear>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.2f, 0.6f) * Projectile.scale);
			Main.dust[dust0].noGravity = true;
		}
		for (int j = 0; j < 30; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(1, 3f), 0).RotatedByRandom(6.283) * Projectile.scale * k1;
			int dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<BlueParticleDark2>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.7f, 2.5f));
			Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 50);
			Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);
		}
		foreach (NPC target in Main.npc)
		{
			float Dis = (target.Center - Projectile.Center).Length();

			if (Dis < k0 * 50)
			{
				if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy() && target.active)
				{
					NPC.HitModifiers npcHitM = new NPC.HitModifiers();
					NPC.HitInfo hit = npcHitM.ToHitInfo(Projectile.damage / (Dis + 35f) * 35f, Main.rand.NextFloat(100f) < Main.player[Projectile.owner].GetTotalCritChance(Projectile.DamageType), 0.2f);
					target.StrikeNPC(hit, true, true);
					NetMessage.SendStrikeNPC(target, hit);
				}
			}
		}
	}
}
