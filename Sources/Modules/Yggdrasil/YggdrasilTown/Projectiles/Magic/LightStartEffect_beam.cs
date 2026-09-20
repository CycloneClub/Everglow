using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class LightStartEffect_beam : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.DamageType = DamageClass.Default;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 1200;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.ai[0] = 0;
	}

	public override void AI()
	{
		if (Projectile.timeLeft is > 1194)
		{
			Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 3, 0.1f);
			for (int i = 0; i < 3; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LampWood_Dust_fluorescent_appear>());
				dust.alpha = 0;
				dust.rotation = Main.rand.NextFloat(0.3f, 0.7f);
				dust.scale *= 2f;
				dust.velocity = new Vector2(0, Main.rand.NextFloat(12f)).RotatedByRandom(6.283f);
			}
		}
		else if (Projectile.timeLeft is > 1170)
		{
			Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 0.4f, 0.3f);
		}
		else if (Projectile.timeLeft is > 120)
		{
			Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], (Math.Sin(Main.time * 0.03f + Projectile.whoAmI * 2) * 0.4f + 1f) * 0.5f, 0.3f);
		}
		else
		{
			Projectile.ai[0] *= 0.96f;
		}
		GenerateParticles(3);
		Projectile.velocity *= 0;
		Lighting.AddLight(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 20, new Vector3(0.8f, 0.6f, 0) * Projectile.ai[0] * 3);
	}

	public void GenerateParticles(int duplicateTimes = 1)
	{
		float mulMaxTime = 1f;
		if (Projectile.timeLeft > 1100)
		{
			mulMaxTime = 1f - (Projectile.timeLeft - 1100) / 100f;
		}
		if (Projectile.timeLeft < 150)
		{
			mulMaxTime = (Projectile.timeLeft - 90f) / 60f;
		}
		if (mulMaxTime < 0)
		{
			return;
		}
		for (int i = 0; i < duplicateTimes; i++)
		{
			Vector2 newVelocity = new Vector2(0, 1.2f).RotatedBy(Main.time * 0.02f + Projectile.whoAmI + (float)i / duplicateTimes * MathHelper.TwoPi);
			var somg = new LightFruitParticleDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(37, 145) * mulMaxTime,
				scale = Main.rand.NextFloat(12.20f, 32.35f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(1, 8f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int i = 0; i < duplicateTimes; i++)
		{
			Vector2 newVelocity = new Vector2(0, 1.2f).RotatedBy(-Main.time * 0.02f + Projectile.whoAmI + (float)i / duplicateTimes * MathHelper.TwoPi);
			var somg = new LightFruitParticleDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(37, 145) * mulMaxTime,
				scale = Main.rand.NextFloat(12.20f, 32.35f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(1, 8f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texMain = Commons.ModAsset.StarSlash.Value;
		var drawColor = new Color(0.5f, 0.4f, 0.2f, 0f);
		float timeValue = (float)Main.time * 0.03f;
		float scale = Projectile.scale * Projectile.ai[0];
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, 0, texMain.Size() / 2f, scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, MathHelper.PiOver2, texMain.Size() / 2f, scale, SpriteEffects.None, 0);

		drawColor = new Color(0.3f, 0.2f, 0.1f, 0f);
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, -MathHelper.PiOver4, texMain.Size() / 2f, new Vector2(0.3f, scale * 0.7f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, MathHelper.PiOver4, texMain.Size() / 2f, new Vector2(0.3f, scale * 0.7f), SpriteEffects.None, 0);

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float radiusScale = (20 + scale) / 20f;
		var bars = new List<Vertex2D>();
		for (int i = 0; i <= 40; i++)
		{
			bars.Add(drawPos + new Vector2(0, -30 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), Color.Transparent, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f, 0));
			bars.Add(drawPos + new Vector2(0, -70 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), drawColor * scale, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f + 0.1f, 0));
		}
		for (int i = 0; i <= 40; i++)
		{
			bars.Add(drawPos + new Vector2(0, -70 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), drawColor * scale, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f + 0.1f, 0));
			bars.Add(drawPos + new Vector2(0, -120 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), Color.Transparent, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f + 0.3f, 0));
		}
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_longitudinalFold.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		drawColor = new Color(0.2f, 0.2f, 0.2f, 0f);
		bars = new List<Vertex2D>();
		for (int i = 0; i <= 40; i++)
		{
			float colorScale = (MathF.Sin(i / 20f * MathHelper.TwoPi + Projectile.whoAmI) + 1) * 0.5f;
			bars.Add(drawPos + new Vector2(0, -30 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), Color.Transparent, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f, 0));
			bars.Add(drawPos + new Vector2(0, -70 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), drawColor * scale * colorScale, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f + 0.6f, 0));
		}
		for (int i = 0; i <= 40; i++)
		{
			float colorScale = (MathF.Sin(i / 20f * MathHelper.TwoPi + Projectile.whoAmI) + 1) * 0.5f;
			bars.Add(drawPos + new Vector2(0, -70 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), drawColor * scale * colorScale, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f + 0.6f, 0));
			bars.Add(drawPos + new Vector2(0, -120 * radiusScale).RotatedBy(i / 40f * MathHelper.TwoPi), Color.Transparent, new Vector3(i / 40f, Projectile.whoAmI - timeValue * 0.1f + 1.2f, 0));
		}
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_SolarSpectrum.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Photolysis>(), 180);
	}
}
