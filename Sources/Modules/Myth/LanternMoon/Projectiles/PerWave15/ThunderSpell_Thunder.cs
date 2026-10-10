using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities.BuffHelpers;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class ThunderSpell_Thunder : ModProjectile, IBloomProjectile
{
	public float Timer = 0;

	public List<Vector3> LightingBoltTrail = new List<Vector3>();

	public List<Vector2> LightingBoltVel = new List<Vector2>();

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 2;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0.75f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Vector2 lightingBoltStart = new Vector2(0, -1000).RotatedByRandom(0.4f);
		int step = 20;
		for (int k = 0; k < step; k++)
		{
			float value = k / (float)step;
			var pos = lightingBoltStart * (1 - value);
			LightingBoltTrail.Add(new Vector3(pos + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(MathHelper.TwoPi), value));
			LightingBoltVel.Add(new Vector2(0, Main.rand.NextFloat(0.35f)).RotatedByRandom(MathHelper.TwoPi));
		}
		LightingBoltTrail.Add(new Vector3(0, 0, 1));
		LightingBoltVel.Add(Vector2.zeroVector);

		for (int k = 0; k < 32; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 24f)).RotatedByRandom(MathHelper.TwoPi);
			var thunderSpark = new ThunderSpellDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				Collided = false,
				MaxTime = Main.rand.Next(75, 90),
				Scale = Main.rand.NextFloat(3f, 8.2f),
			};
			Ins.VFXManager.Add(thunderSpark);
		}
	}

	public override void AI()
	{
		Timer++;
		if (Timer > 10)
		{
			Projectile.hostile = false;
		}
		Projectile.velocity *= 0;
		for (int j = 0; j < LightingBoltTrail.Count; j++)
		{
			Vector2 vel = Vector2.zeroVector;
			if (LightingBoltVel.Count > j)
			{
				vel = LightingBoltVel[j];
			}
			Vector3 v0 = LightingBoltTrail[j] + new Vector3(vel, 0);
			v0.Z *= 0.96f;
			LightingBoltTrail[j] = v0;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		// Main.NewText(info.Damage);
		target.AddBuff(ModContent.BuffType<ShortImmune3>(), 6);
		base.OnHitPlayer(target, info);
	}

	public void DrawLightningBolt()
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		var drawPos = Projectile.Center;
		float fade = 1f;
		if (Projectile.timeLeft < 60)
		{
			fade = Projectile.timeLeft / 60f;
		}
		List<Vertex2D> bars_bolt = new List<Vertex2D>();
		Color lightningColor = Color.Lerp(new Color(1f, 0.9f, 0.4f, 0), new Color(0.3f, 0.15f, 0.02f, 0), 1 - fade);
		for (int i = 0; i < LightingBoltTrail.Count; i++)
		{
			Vector2 pos = new Vector2(LightingBoltTrail[i].X, LightingBoltTrail[i].Y);
			Vector2 dir = -new Vector2(LightingBoltTrail[0].X, LightingBoltTrail[0].Y);
			if (i < LightingBoltTrail.Count - 1)
			{
				Vector2 posNext = new Vector2(LightingBoltTrail[i + 1].X, LightingBoltTrail[i + 1].Y);
				dir = posNext - pos;
			}
			dir = dir.NormalizeSafe();
			Vector2 width = dir.RotatedBy(MathHelper.PiOver2) * 16;
			float mulColor = 1;
			bars_bolt.Add(drawPos + pos + width, lightningColor * mulColor, new Vector3(i / 30f, 0, LightingBoltTrail[i].Z));
			bars_bolt.Add(drawPos + pos - width, lightningColor * mulColor, new Vector3(i / 30f, 1, LightingBoltTrail[i].Z));
			if (i % 6 == 0)
			{
				Lighting.AddLight(drawPos + pos, new Vector3(1f, 0.9f, 0.4f) * fade * mulColor * LightingBoltTrail[i].Z * 1.5f);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect1 = ModAsset.CylindricalLantern_explosion_lightningbolt.Value;
		effect1.Parameters["uTransform"].SetValue(model * projection);
		effect1.CurrentTechnique.Passes[0].Apply();
		if (bars_bolt.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_7.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_bolt.ToArray(), 0, bars_bolt.Count - 2);
			if (Timer < 10)
			{
				for (int k = 0; k < 10 - Timer; k++)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_bolt.ToArray(), 0, bars_bolt.Count - 2);
				}
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, lightningColor, 0, star.Size() * 0.5f, new Vector2(fade, 1f), SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, lightningColor, -MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(fade, 0.75f), SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, lightningColor, MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(fade, 0.75f), SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, lightningColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(fade, 2f), SpriteEffects.None, 0);
		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.EntitySpriteDraw(spot, Projectile.Center - Main.screenPosition, null, lightningColor, 0, spot.Size() * 0.5f, fade * 2.4f, SpriteEffects.None, 0);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawLightningBolt();
		return false;
	}

	public void DrawBloom()
	{
		DrawLightningBolt();
	}
}
