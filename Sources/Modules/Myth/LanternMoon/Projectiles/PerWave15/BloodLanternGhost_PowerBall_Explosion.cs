using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.VFX;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class BloodLanternGhost_PowerBall_Explosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public float Timer = 0;

	public struct LightningBolt()
	{
		public List<Vector3> Joint;
		public int Timer;
		public int MaxTime;
		public bool Active;
		public float[] ai;
	}

	public List<LightningBolt> LightningBolts = new List<LightningBolt>();

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 48;
		Projectile.height = 48;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Wave(Projectile.Center);
	}

	public override void AI()
	{
		Timer++;
		Projectile.velocity *= 0;
		UpdateLightningBolts();
	}

	public void UpdateLightningBolts()
	{
		if (Timer < 5)
		{
			for (int i = 0; i < 6; i++)
			{
				Vector2 pos = Vector2.zeroVector;
				Vector2 vel = new Vector2(0, 8).RotatedByRandom(MathHelper.TwoPi);
				AddLightningBolt(pos, vel, 1);
			}
		}
		else
		{
			if (Projectile.timeLeft > 20)
			{
				Vector2 pos = Vector2.zeroVector;
				Vector2 vel = new Vector2(0, 8).RotatedByRandom(MathHelper.TwoPi);
				AddLightningBolt(pos, vel, 1);
			}
		}
		for (int k = LightningBolts.Count - 1; k >= 0; k--)
		{
			LightningBolt bolt = LightningBolts[k];
			if (bolt.Active)
			{
				bolt.Timer++;
				if (bolt.Timer > bolt.MaxTime)
				{
					bolt.Active = false;
				}
				for (int i = 0; i < bolt.Joint.Count; i++)
				{
					Vector2 pos = new Vector2(bolt.Joint[i].X, bolt.Joint[i].Y);
					if (i > 0)
					{
						pos += new Vector2(0, 0.25f).RotatedByRandom(MathHelper.TwoPi);
					}
					bolt.Joint[i] = new Vector3(pos, bolt.Joint[i].Z * 0.8f);
				}
				LightningBolts[k] = bolt;
			}
			else
			{
				LightningBolts.Remove(bolt);
			}
		}
	}

	public void AddLightningBolt(Vector2 position, Vector2 velocity, float size)
	{
		LightningBolt bolt = new LightningBolt();
		bolt.Joint = new List<Vector3>();
		bolt.Active = true;
		bolt.Timer = 0;
		bolt.MaxTime = 15;
		float totalRot = 0;
		for (int i = 0; i < 30; i++)
		{
			bolt.Joint.Add(new Vector3(position, size));
			size *= 0.96f;
			position += velocity;
			float rot = Main.rand.NextFloat(-0.3f, 0.3f);
			rot -= totalRot * 0.3f;
			velocity = velocity.RotatedBy(rot);
			totalRot += rot;
			if (Main.rand.NextBool(9) && size > 0.2f)
			{
				float rot2 = Main.rand.NextFloat(-0.7f, 0.7f);
				Vector2 vel2 = velocity.RotatedBy(-rot2);
				velocity = velocity.RotatedBy(rot2);
				size *= 0.5f;
				AddLightningBolt(position, vel2, size * 0.5f);
			}
		}
		LightningBolts.Add(bolt);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float disValue = MathF.Sqrt(Timer) * 22;
		bool CheckCenter(Vector2 pos)
		{
			return (pos - projHitbox.Center()).Length() < disValue / 0.9f;
		}
		return CheckCenter(targetHitbox.TopLeft()) || CheckCenter(targetHitbox.TopRight()) || CheckCenter(targetHitbox.BottomLeft()) || CheckCenter(targetHitbox.BottomRight());
	}

	public void Wave(Vector2 pos)
	{
		if (Ins.VisualQuality.High)
		{
			var wave = new WarpLanternWave
			{
				Position = pos,
				Speed = 12f * Projectile.ai[0],
				Range = 0,
				Timer = 0,
				MaxTime = 30,
				SpeedDecay = 0.9f,
				Active = true,
				Visible = true,
			};
			Ins.VFXManager.Add(wave);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var drawPos = Projectile.Center;
		float disValue = MathF.Sqrt(Timer) * 22;
		float fade = 1f;
		if (Projectile.timeLeft < 60)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		float fade2 = 1f;
		if (Projectile.timeLeft < 30)
		{
			fade2 *= Projectile.timeLeft / 30f;
		}
		var drawColor = Color.Lerp(new Color(1f, 1f, 0.4f, 0), new Color(1f, 0.05f, 0.1f, 0), 1 - fade) * (MathF.Sin(Timer * 0.9f) * 0.55f * fade + 1);
		float warp = 1 - fade;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_dark = new List<Vertex2D>();
		List<Vertex2D> bars_side = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue).RotatedBy(i / 60f * MathHelper.TwoPi + Projectile.whoAmI);
			Vector2 radius_out = radius * 2f;
			bars_dark.Add(drawPos + radius, Color.White * fade, new Vector3(i / 15f, 0.35f, 0));
			bars_dark.Add(drawPos + radius_out, Color.White * fade, new Vector3(i / 15f, 0, warp));
			bars.Add(drawPos + radius, drawColor * fade, new Vector3(i / 15f, 0.35f, 0));
			bars.Add(drawPos + radius_out, drawColor * fade * 0.5f, new Vector3(i / 15f, 0, 0));
			bars_side.Add(drawPos + radius * 0.6f, drawColor * fade2 * 2, new Vector3(i / 60f * 8, 1f, 0));
			bars_side.Add(drawPos + radius * 1.4f, drawColor * fade2 * 2, new Vector3(i / 60f * 8, 0, warp));
			Lighting.AddLight(drawPos + radius, new Vector3(1f, 1f, 0.1f) * fade);
		}

		List<Vertex2D> bars_bolt = new List<Vertex2D>();
		foreach (var bolt in LightningBolts)
		{
			Color lightningColor = Color.Lerp(new Color(1f, 0.6f, 0.4f, 0), new Color(1f, 0.05f, 0.1f, 0), bolt.Timer / (float)bolt.MaxTime);
			for (int i = 0; i < bolt.Joint.Count - 1; i++)
			{
				Vector2 pos = new Vector2(bolt.Joint[i].X, bolt.Joint[i].Y);
				Vector2 posNext = new Vector2(bolt.Joint[i + 1].X, bolt.Joint[i + 1].Y);
				Vector2 dir = posNext - pos;
				dir = dir.NormalizeSafe();
				Vector2 width = dir.RotatedBy(MathHelper.PiOver2) * 8;
				float mulColor = 1;
				if (i == 0)
				{
					mulColor *= 0;
				}
				if (pos.Length() > disValue)
				{
					mulColor *= 0f;
				}
				bars_bolt.Add(drawPos + pos + width, lightningColor * mulColor, new Vector3(i / 30f, 0, bolt.Joint[i].Z));
				bars_bolt.Add(drawPos + pos - width, lightningColor * mulColor, new Vector3(i / 30f, 1, bolt.Joint[i].Z));
				if (i % 6 == 0)
				{
					Lighting.AddLight(drawPos + pos, new Vector3(1f, 1f, 0.1f) * fade * mulColor);
				}
			}
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect0 = ModAsset.LanternGhostKing_Matrix_Shader.Value;
		effect0.Parameters["uTransform"].SetValue(model * projection);
		effect0.Parameters["size1"].SetValue(Vector2.One);
		effect0.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
		effect0.Parameters["warpScale"].SetValue(1f);
		effect0.CurrentTechnique.Passes[0].Apply();

		if (bars.Count > 0 && bars_dark.Count > 0 && bars_side.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_rgb_large.Value;
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.LanternGhostKing_Matrix_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.LanternGhostKing_Matrix.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_side.ToArray(), 0, bars_side.Count - 2);
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
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float star_width = 2f;
		if (Projectile.timeLeft < 60)
		{
			star_width *= Projectile.timeLeft / 60f;
		}
		star_width *= 1 + (MathF.Sin((float)Main.time * 0.23f + Projectile.whoAmI) + 0.5f) * 0.7f;
		float timeValue = MathF.Sin((float)Main.time * 0.07f + Projectile.whoAmI) * 0.5f + 0.5f;
		Color c0 = new Color(1f, 0.75f * timeValue, 0, 0) * timeValue;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, MathHelper.PiOver2, star.Size() / 2f, new Vector2(star_width / 1.5f, 0.5f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, 0, star.Size() / 2f, new Vector2(star_width / 1.5f, 0.5f), SpriteEffects.None, 0);

		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, MathHelper.PiOver4, star.Size() / 2f, star_width / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, c0, -MathHelper.PiOver4, star.Size() / 2f, star_width / 3f, SpriteEffects.None, 0);

		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(spot, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 0.7f, 0), (float)Main.timeForVisualEffects * 0.04f, spot.Size() / 2f, star_width, SpriteEffects.None, 0);
		return false;
	}
}
