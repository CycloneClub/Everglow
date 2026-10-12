using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Dusts;
using Terraria.Audio;

namespace Everglow.SpellAndSkull.Projectiles.CrystalStorm;

public class LargeCrystal : ModProjectile// This proj summon storm at breaking
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
		Projectile.extraUpdates = 1;
		Projectile.timeLeft = 1000;
		Projectile.alpha = 0;
		Projectile.penetrate = 2;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Magic;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
	}

	private int timeToKill = 0;

	public override void AI()
	{
		timeToKill--;
		if (timeToKill < 0)
		{
			Projectile.velocity *= 0.9993f;
			Projectile.velocity.Y += 0.02f;
			float k0 = 1f / (Projectile.ai[0] + 2) * 2;
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * Projectile.scale * 0.3f;
			Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<CrystalAppearStoppedByTile>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * Projectile.scale * 0.4f * k0);
			if (Projectile.ai[1] > 0)
			{
				Projectile.ai[1] -= 1;
				if (Projectile.ai[1] == 1)
				{
					Projectile.Kill();
				}
			}
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		}
		else
		{
			Projectile.velocity *= 0;
			if (timeToKill == 0)
			{
				Projectile.Kill();
			}
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		HitTile();
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		HitTile();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D light = ModAsset.LargeCrystal.Value;
		float k1 = (100f + Projectile.ai[0] * 25) * 0.3f;
		float k0 = (1000 - Projectile.timeLeft) / k1;
		float k2 = 1f;
		if (Projectile.timeLeft <= 1000 - k1)
		{
			k0 = 1;
		}

		if (Projectile.timeLeft < 200)
		{
			k2 = Projectile.timeLeft / 200f;
		}

		var bars = new List<Vertex2D>();
		float width = 24;
		var c0 = new Color(k0 * k0 * 1.6f, 0, k0 * 0.4f + 0.2f, 0);
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		for (int i = 1; i < trueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			if (i < 31)
			{
				float k3 = (i - 1) / 30f;
				c0 = new Color(k0 * k0 * 0.9f * k3 * (1 - k3), k0 * 0.06f, k0 * 0.1f * k3 + 0.06f * k3, 0);
			}
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Utils.SafeNormalize(new Vector2(-normalDir.Y, normalDir.X), Vector2.zeroVector);
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 0, w)));
		}
		Texture2D t = ModAsset.BladeShadow.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (timeToKill < 0)
		{
			Color c1 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
			c1.A = 200;
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, c1, Projectile.rotation, light.Size() / 2f, (k0 / 1.8f + 0.2f) / (Projectile.ai[0] + 3) * 3.5f * k2, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, c0, Projectile.rotation, light.Size() / 2f, (k0 / 1.8f + 0.2f) / (Projectile.ai[0] + 3) * 3.5f * k2, SpriteEffects.None, 0);
		}

		return false;
	}

	public void HitTile()
	{
		var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Storm>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystalExplosion>(), (int)(Projectile.damage * 2.3f), Projectile.knockBack, Projectile.owner, 14);
		for (int j = 0; j < 120; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * Projectile.scale;
			int dust0 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<CrystalAppearStoppedByTile>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.3f, 0.6f) * Projectile.scale * 2);
			Main.dust[dust0].noGravity = true;
			Main.dust[dust0].dustIndex = p.whoAmI;
			var cp = new CrystalParticle
			{
				timeLeft = 70,
				size = Main.rand.NextFloat(0.45f, 1.55f),
				velocity = new Vector2(Main.rand.NextFloat(2.5f, 7.5f), 0).RotatedByRandom(6.283),
				Active = true,
				Visible = true,
				position = Projectile.Center,
			};

			Ins.VFXManager.Add(cp);
		}
		for (int j = 0; j < 15; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(2f, 7f)).RotatedByRandom(6.28);
			int ds = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + v * 3f + Projectile.velocity, v, ProjectileID.CrystalStorm, Projectile.damage / 3, 1, Projectile.owner, 0);
			Main.projectile[ds].scale = Main.rand.NextFloat(0.85f, 1.25f);
		}
		float x0 = Main.rand.NextFloat(0, 6.283f);
		for (int j = 0; j < 40; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedBy(Main.rand.NextFloat(3.14159f, 6.283f) + x0) * Projectile.scale;
			int dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<CrystalParticleDark2StoppedByTile>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f) * 5);
			Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 10);
			Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);

			v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedBy(Main.rand.NextFloat(0, 3.14159f) + x0) * Projectile.scale;
			dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_0>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f) * 5);
			Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 10);
			Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);
		}
		SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.damage = 0;
		timeToKill = 60;
	}
}
