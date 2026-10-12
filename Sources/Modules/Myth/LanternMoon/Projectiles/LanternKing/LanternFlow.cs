using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class LanternFlow : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 650;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
		TrailLength = 400;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 20000;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		TrailColor = new Color(1f, 0.2f, 0f, 0f) * 0.3f;
		TrailBackgroundDarkness = 0.3f;
		TrailWidth = 240f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_2_thick.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		WarpStrength = 1f;
	}

	public NPC OwnerNPC;

	public Vector2 TargetCenter => OwnerNPC != null ? OwnerNPC.Center + new Vector2(0, 50) : Vector2.Zero;

	public override void OnSpawn(IEntitySource source)
	{
		if (OwnerNPC == null)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.active)
				{
					if (npc.type == ModContent.NPCType<LanternGhostKing>())
					{
						OwnerNPC = npc;
					}
				}
			}
		}
	}

	public float MinDisToNPC;

	public float BestVelDecay;

	public float BestRotateSpeed;

	public float VelDecay;

	public float RotateSpeed;

	public override void Behaviors()
	{
		if (OwnerNPC == null)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.active)
				{
					if (npc.type == ModContent.NPCType<LanternGhostKing>())
					{
						OwnerNPC = npc;
					}
				}
			}
		}
		if (OwnerNPC == null)
		{
			Projectile.active = false;
			return;
		}
		if (Projectile.timeLeft > 507)
		{
			Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.timeLeft * 0.08f) * 0.02f);
		}
		else if (Projectile.timeLeft > 472)
		{
			Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]);
		}
		else
		{
			// float acceleration = toOwner.Length() + MathF.Sqrt(MathF.Abs(toOwner.LengthSquared() - Projectile.velocity.LengthSquared()));
			// Projectile.velocity += toOwner.NormalizeSafe() * acceleration;
			Projectile.velocity *= VelDecay;
			Projectile.velocity = Projectile.velocity.RotatedBy(RotateSpeed);
			if (Projectile.timeLeft < 220)
			{
				TrailColor *= 0.99f;
				TrailBackgroundDarkness *= 0.98f;
				WarpStrength *= 0.98f;
			}
		}

		// Particles
		float upBound = Math.Min(Timer * 1.4f - 2, SmoothedOldPos.Count - 2);
		if (Projectile.timeLeft < 580)
		{
			if (upBound > 5)
			{
				for (int i = 0; i < SmoothedOldPos.Count / 240; i++)
				{
					int checkOldPosIndex = Main.rand.Next(5, (int)upBound);
					checkOldPosIndex = Math.Clamp(checkOldPosIndex, 0, SmoothedOldPos.Count - 1);
					float mulScale = Main.rand.NextFloat(0.5f, 1.2f);
					if (Projectile.timeLeft < 120f)
					{
						mulScale *= Projectile.timeLeft / 120f;
					}
					Vector2 addPos = new Vector2(Main.rand.NextFloat(0f, 90f), 0).RotateRandom(MathHelper.TwoPi);
					var gore2 = new LanternFlow_lantern
					{
						Active = true,
						Visible = true,
						velocity = -Vector2.Normalize(SmoothedOldPos.ToArray()[checkOldPosIndex] - SmoothedOldPos.ToArray()[checkOldPosIndex - 1]) * mulScale * 6 - addPos * 0.01f,
						scale = mulScale,
						position = SmoothedOldPos.ToArray()[checkOldPosIndex] + addPos,
						npcOwner = OwnerNPC,
					};
					Ins.VFXManager.Add(gore2);
				}
			}
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override void DrawTrail()
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = TrailShader;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();

		// Draw black background first.
		Main.graphics.GraphicsDevice.Textures[0] = TrailTextureBlack;
		var bars0 = new List<Vertex2D>();
		var bars1 = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();

		int countSafe = 0;
		float darknessRemainder = TrailBackgroundDarkness;
		while (darknessRemainder > 0)
		{
			float darkValue = 1;
			if (darknessRemainder < 1f)
			{
				darkValue = darknessRemainder;
			}
			CreateTrailVertex(bars0, bars1, bars2, 0, default, darkValue);
			if (bars0.Count >= 2 && bars1.Count >= 2 && bars2.Count >= 2)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars0.ToArray(), 0, bars0.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars1.ToArray(), 0, bars1.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
			}
			darknessRemainder -= 1f;
			countSafe++;
			if (countSafe > 100)
			{
				break;
			}
		}

		var bars3 = new List<Vertex2D>();
		CreateNormalVertex(bars3, 0);
		if (bars3.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_19_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}

		// Then, draw the colorful trail.
		Main.graphics.GraphicsDevice.Textures[0] = TrailTexture;
		bars0.Clear();
		bars1.Clear();
		bars2.Clear();
		CreateTrailVertex(bars0, bars1, bars2, 1);
		if (bars0.Count >= 2 && bars1.Count >= 2 && bars2.Count >= 2)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars0.ToArray(), 0, bars0.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars1.ToArray(), 0, bars1.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}

		bars3 = new List<Vertex2D>();
		CreateNormalVertex(bars3, 1);
		if (bars3.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_19.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void CreateNormalVertex(List<Vertex2D> bars, int style)
	{
		// If there is no any element here, return.
		if (SmoothedOldPos.Count <= 0 || TrailLength <= 0)
		{
			return;
		}

		// If dark, the Color.White will be proper.
		for (int i = 1; i < SmoothedOldPos.Count; ++i)
		{
			// factor, among 0 to 1, usually for deciding the trail's texture.coord.X.
			float mulFac = Timer / (float)TrailLength;
			if (mulFac > 1f)
			{
				mulFac = 1f;
			}
			float factor = (i + 1) / (float)SmoothedOldPos.Count * mulFac;

			float width = 0;
			if (i >= 0)
			{
				width = TrailWidthFunction(factor);
			}

			// timeValue, animate the trail.
			float timeValue = Timer * 0.01f;
			Vector2 drawPos = Projectile.Center;
			if (i >= 0)
			{
				drawPos = SmoothedOldPos[i];
			}
			var dir = SmoothedOldPos[i - 1] - SmoothedOldPos[i];
			dir = dir.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * TrailWidth * 0.5f;
			if (style == 1)
			{
				style = 4;
			}
			Color drawColor = GetTrailColor(style, drawPos, i, ref factor, 1, 0);
			bars.Add(drawPos + dir, drawColor, new Vector3(factor * 24 + timeValue, 0, width));
			bars.Add(drawPos - dir, drawColor, new Vector3(factor * 24 + timeValue, 1, width));
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		float disValue = ((worldPos - TargetCenter).Length() - 150) / 100f;
		if (style == 0)
		{
			Color c0 = base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
			if (disValue < 1)
			{
				c0 *= disValue;
			}
			return c0;
		}
		if (style == 1)
		{
			Color c0 = base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
			disValue = ((worldPos - TargetCenter).Length() - 180) / 100f;
			disValue = MathF.Abs(disValue);
			disValue = 1 - disValue;
			disValue = Math.Max(disValue, 0.3f);
			disValue = MathF.Pow(disValue, 1.5f);
			c0 *= disValue;
			return c0;
		}
		if (style == 4)
		{
			float value = MathF.Sin((worldPos.X + worldPos.Y + (float)Main.time * 15) * 0.01f) * 0.5f + 0.5f;
			Color c0 = Color.Lerp(new Color(0.5f, 0f, 0f, 0f), new Color(0.85f, 0.2f, 0.04f, 0f), value);
			if (disValue < 1)
			{
				c0 *= disValue;
			}
			return c0;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override void DrawSelf()
	{
		float colorValue = 1f;
		if (Projectile.timeLeft < 100)
		{
			colorValue = (Projectile.timeLeft - 80) / 20f;
		}
		if (Projectile.timeLeft < 80)
		{
			return;
		}
		Texture2D star = Commons.ModAsset.StarSlashGray.Value;
		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Color drawColor = new Color(1f, 0.2f + 0.2f * MathF.Sin((float)Main.time * 0.03f + Projectile.whoAmI), 0, 0) * colorValue;
		var drawPos = Projectile.Center - Main.screenPosition;
		Main.EntitySpriteDraw(star, drawPos, null, drawColor, 0, star.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, drawPos, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(spot, drawPos, null, new Color(1f, 0.8f, 0.7f, 0) * colorValue, 0, spot.Size() * 0.5f, 1f, SpriteEffects.None, 0);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor * 4;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float wid = 0.3f;
		if (Projectile.timeLeft < 300)
		{
			wid *= Projectile.timeLeft / 300f;
		}
		for (int i = 0; i < TrailPos.Count(); i++)
		{
			if ((targetHitbox.Center() - (TrailPos.ToArray()[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f)).Length() < TrailWidth * wid)
			{
				return true;
			}
		}
		return false;
	}
}
