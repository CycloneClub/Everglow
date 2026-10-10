using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class ToothSpear_proj : ModProjectile
{
	public virtual float HoldoutRangeMin => 24f;

	public virtual float HoldoutRangeMax => 150f;

	public Vector2 LockCenter = Vector2.Zero;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		GenerateVFX_hitNPC(target, 8);
	}

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.Spear);
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
	}

	public override bool PreAI()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI / 4f;
		Player player = Main.player[Projectile.owner];
		int duration = player.itemAnimationMax;
		player.heldProj = Projectile.whoAmI;

		if (Projectile.timeLeft > duration)
		{
			Projectile.timeLeft = duration;
		}

		Projectile.velocity = Vector2.Normalize(Projectile.velocity);

		float halfDuration = duration * 0.5f;
		float progress;
		int dir = 1;
		if (Projectile.timeLeft < halfDuration)
		{
			GenerateVFX(dir);
			progress = Projectile.timeLeft / halfDuration;
		}
		else
		{
			dir = -1;
			if (Main.rand.NextBool(8))
			{
				GenerateVFX(dir);
			}
			progress = (duration - Projectile.timeLeft) / halfDuration;
		}
		Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

		return false;
	}

	public void GenerateVFX(int direction)
	{
		Player player = Main.player[Projectile.owner];
		for (int g = 0; g < 1; g++)
		{
			Vector2 afterVelocity = Projectile.velocity * Main.rand.NextFloat(4f, 9f) * direction * player.meleeSpeed;
			float mulScale = Main.rand.NextFloat(6f, 25f);
			var blood = new BloodDrop
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(82, 164),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < 1; g++)
		{
			Vector2 afterVelocity = Projectile.velocity * 2f * direction * player.meleeSpeed;
			var blood = new BloodSplash
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(42, 78),
				Scale = Main.rand.NextFloat(6f, 18f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public void GenerateVFX_hitNPC(NPC target, float times = 1)
	{
		for (int g = 0; g < times; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(8f)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(6f, 25f);
			var blood = new BloodDrop
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = target.Center,
				MaxTime = Main.rand.Next(82, 164),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < times; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(8f)).RotatedByRandom(MathHelper.TwoPi);
			var blood = new BloodSplash
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = target.Center,
				MaxTime = Main.rand.Next(42, 78),
				Scale = Main.rand.NextFloat(6f, 18f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D mainTex = ModAsset.ToothSpear_proj.Value;
		Vector2 drawCenter = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 75f;
		Main.spriteBatch.Draw(mainTex, drawCenter - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		drawCenter = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 20f;
		float timeValue = 1f;
		int duration = player.itemAnimationMax;
		float halfDuration = duration * 0.5f;
		if (Projectile.timeLeft < halfDuration * 0.2f + 10)
		{
			timeValue = (Projectile.timeLeft - halfDuration * 0.2f) / 10f;
		}
		if (Projectile.timeLeft < halfDuration * 0.2f)
		{
			timeValue = 0;
		}
		if (Projectile.timeLeft >= halfDuration)
		{
			LockCenter = drawCenter;
		}
		Vector2 vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathF.PI * 0.5) * 90 * timeValue;
		Color drawColor = lightColor;
		drawColor.G = 0;
		drawColor.B = 0;
		drawColor.A = 0;
		int trailLength = 16;
		if (duration - Projectile.timeLeft < 8 / player.meleeSpeed)
		{
			trailLength = (int)((duration - Projectile.timeLeft) * 2 * player.meleeSpeed);
		}
		float timeEffectValue = (float)(Main.time * 0.10f);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 6)
			{
				value = (trailLength - 1 - x) / 5f;
			}
			bars.Add(LockCenter - vel * 12 * x + width, drawColor * value, new Vector3(x / 15f - timeEffectValue, 1, MathF.Sin(x / 16f)));
			bars.Add(LockCenter - vel * 12 * x - width, drawColor * value, new Vector3(x / 15f - timeEffectValue, 0, MathF.Sin(x / 16f)));
		}

		List<Vertex2D> barsDark = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 6)
			{
				value = (trailLength - 1 - x) / 5f;
			}
			barsDark.Add(LockCenter - vel * 12 * x + width, Color.White * value, new Vector3(x / 15f - timeEffectValue, 1, MathF.Sin(x / 16f)));
			barsDark.Add(LockCenter - vel * 12 * x - width, Color.White * value, new Vector3(x / 15f - timeEffectValue, 0, MathF.Sin(x / 16f)));
		}
		drawColor = lightColor;
		List<Vertex2D> barsHighLight = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 12)
			{
				value = (trailLength - 1 - x) / 11f;
			}
			drawColor.A = (byte)(value * 255);
			barsHighLight.Add(LockCenter - vel * 6 * x + width * 0.4f, drawColor, new Vector3(x / 30f - timeEffectValue, 1, MathF.Sin(x / 8f)));
			barsHighLight.Add(LockCenter - vel * 6 * x - width * 0.4f, drawColor, new Vector3(x / 30f - timeEffectValue, 0, MathF.Sin(x / 8f)));
		}

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = Commons.ModAsset.StabSwordEffect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uProcession"].SetValue(0.5f);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		effect = Commons.ModAsset.Trailing.Value;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes["HeatMap"].Apply();

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		Main.graphics.graphicsDevice.Textures[1] = ModAsset.TuskKnife_meleeColor.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsHighLight.ToArray(), 0, barsHighLight.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
