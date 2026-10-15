using Everglow.Commons.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee.EvilHalbertBarnacle;

public class EvilHalbertBarnacle_proj_Thrust : ModProjectile
{
	public int State = 0;

	public override string Texture => ModAsset.EvilHalbertBarnacle_proj_Mod;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public float HoldoutRangeMin => -24f;

	public float HoldoutRangeMax => 162f;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 40;
	}

	public override bool PreAI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3;
		if (Projectile.velocity.X > 0)
		{
			Projectile.spriteDirection = 1;
		}
		else
		{
			Projectile.spriteDirection = -1;
			Projectile.rotation -= MathHelper.PiOver2;
		}
		Player player = Main.player[Projectile.owner];
		int duration = (int)(14 / player.meleeSpeed);

		player.heldProj = Projectile.whoAmI;

		if (Projectile.timeLeft > duration)
		{
			Projectile.timeLeft = duration;
		}

		Projectile.velocity = Vector2.Normalize(Projectile.velocity);

		float halfDuration = duration * 0.5f;
		float progress;

		if (Projectile.timeLeft < halfDuration)
		{
			progress = Projectile.timeLeft / halfDuration;
		}
		else
		{
			progress = (duration - Projectile.timeLeft) / halfDuration;
		}

		Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

		if (player.ownedProjectileCounts[ModContent.ProjectileType<EvilHalbertBarnacle_proj_shuttle>()] <= 0)
		{
			State = 0;
		}
		else
		{
			State = 1;
		}
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int i = 0; i < 12; i++)
		{
			Rectangle rec = projHitbox;
			rec.X += (int)(Projectile.velocity.X * i * 10);
			rec.Y += (int)(Projectile.velocity.Y * i * 10);
			if (targetHitbox.Intersects(rec))
			{
				return true;
			}
		}
		return false;
	}

	public Vector2 oldPos;

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		if (State == 1)
		{
			tex = ModAsset.EvilHalbertBarnacle_proj_released.Value;
		}
		Vector2 velNor2 = Projectile.velocity.NormalizeSafe() * tex.Width / MathF.Sqrt(2f);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition - velNor2, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Vector2 drawCenter = Projectile.Center;
		float timeValue = 1f;
		if (Projectile.timeLeft < 4)
		{
			timeValue = Projectile.timeLeft / 4f;
		}
		Vector2 vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathHelper.PiOver2) * 60 * timeValue;
		float timeEffectValue = (float)(Main.time * 0.06f) + Projectile.whoAmI * 0.27f;

		Player player = Main.player[Projectile.owner];
		int duration = (int)(14 / player.meleeSpeed);
		float progress2;
		float halfDuration = duration * 0.5f;
		if (Projectile.timeLeft < halfDuration)
		{
			progress2 = 2 - Projectile.timeLeft / halfDuration;
			drawCenter = player.MountedCenter + Vector2.Lerp(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress2 * 0.5f + 1.5f * 0.5f);
		}
		else
		{
			progress2 = (duration - Projectile.timeLeft) / halfDuration;
		}
		if (progress2 <= 0)
		{
			return;
		}
		float unitDistance = (HoldoutRangeMax - HoldoutRangeMin) / 19f * progress2 * 1.4f;

		// dark bottom
		List<Vertex2D> bars = new List<Vertex2D>();

		// light cover
		List<Vertex2D> bars2 = new List<Vertex2D>();

		// light cover 3
		List<Vertex2D> bars3 = new List<Vertex2D>();
		for (int x = 0; x < 20; x++)
		{
			Vector2 drawPos = drawCenter - vel * unitDistance * x;
			Color drawColor = Lighting.GetColor(drawPos.ToTileCoordinates());
			drawColor.A = 255;
			float mulWidth = MathF.Sin(x / 23f * MathHelper.PiOver2);
			float value = 19 - x;
			float fade = 1;
			if (value < 5)
			{
				fade *= 0.2f;
			}
			int fadeDis = 7;
			if (x > Projectile.timeLeft * 3 - fadeDis)
			{
				fade *= Math.Clamp((Projectile.timeLeft * 3 - x) / (float)fadeDis, 0, 1);
			}
			drawColor *= fade;
			bars.Add(drawPos + width, drawColor, new Vector3(-x / 20f - timeEffectValue, 1, mulWidth));
			bars.Add(drawPos - width, drawColor, new Vector3(-x / 20f - timeEffectValue, 0, mulWidth));

			drawColor.A = 0;
			drawColor *= 0.7f;
			bars2.Add(drawPos + width, drawColor, new Vector3(-x / 20f - timeEffectValue, 1, mulWidth));
			bars2.Add(drawPos - width, drawColor, new Vector3(-x / 20f - timeEffectValue, 0, mulWidth));

			drawColor = Lighting.GetColor(drawPos.ToTileCoordinates());
			drawColor.A = 0;
			fade = 19 - x;
			if (value < 5)
			{
				fade *= 0.2f;
			}
			fadeDis = 150;
			if (x > Projectile.timeLeft * 1 - fadeDis)
			{
				fade *= Math.Max(0, (Projectile.timeLeft * 1 - x) / (float)fadeDis);
			}
			drawColor *= fade;
			drawColor *= 2;
			bars3.Add(drawPos + width, drawColor, new Vector3(-x / 20f - timeEffectValue, 1, mulWidth));
			bars3.Add(drawPos - width, drawColor, new Vector3(-x / 20f - timeEffectValue, 0, mulWidth));
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = ModAsset.EvilHalbertBarnacle_proj_Thrust.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes["DarkEffect"].Apply();

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_8.Value;
		Main.graphics.graphicsDevice.Textures[1] = ModAsset.Barnacle_Color.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

		if (bars.Count > 0)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes["LightEffect"].Apply();

		if (bars2.Count > 0 && bars3.Count > 0)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
