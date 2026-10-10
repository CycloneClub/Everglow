using Everglow.Commons.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class WheelShapeLantern3Layer : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public struct MovingEntity()
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public float Scale;
		public float Rotation;
		public float Alpha;
		public Queue<Vector2> OldPositions;
		public int TimeLeft;
		public bool Active;
		public float[] ai;
	}

	public List<MovingEntity> ProjEntities = new List<MovingEntity>();

	public override string Texture => ModAsset.DarkLantern_BodyAndHighlight_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 2000;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		ProjEntities = new List<MovingEntity>();
		Projectile.velocity = Vector2.zeroVector;
	}

	public float Timer = 0;

	public void UpdateEntity()
	{
		if (Timer is 30 or 60 or 90)
		{
			float frequency = 6;
			if (Main.expertMode)
			{
				frequency = 9;
			}
			if (Timer is 60)
			{
				frequency = 9;
				if (Main.expertMode)
				{
					frequency = 15;
				}
			}
			if (Timer is 90)
			{
				frequency = 15;
				if (Main.expertMode)
				{
					frequency = 24;
				}
			}
			float range = 50 + Timer / 30 * 50;
			for (int i = 0; i < frequency; i++)
			{
				MovingEntity entity = new MovingEntity();
				entity.OldPositions = new Queue<Vector2>();
				entity.Position = Projectile.Center + new Vector2(range, 0).RotatedBy(MathHelper.TwoPi * i / frequency);
				entity.Velocity = Vector2.zeroVector;
				entity.Scale = 1;
				entity.Rotation = MathHelper.TwoPi * i / frequency + MathHelper.PiOver2;
				entity.TimeLeft = Main.rand.Next(1500, 1600);
				entity.Active = true;
				entity.Alpha = 1;
				entity.ai = [Main.rand.NextFloat(2.85f, 3.15f), Main.rand.NextFloat(0.85f, 1.15f), 0];
				ProjEntities.Add(entity);
			}
		}

		float accelerationYCoefficient = Timer / 130f;

		for (int i = 0; i < ProjEntities.Count; i++)
		{
			MovingEntity entity = ProjEntities[i];
			if (entity.Active)
			{
				if (Timer < 100)
				{
					entity.Velocity *= 0f;
				}

				// Release
				if (Timer == 100)
				{
					entity.Velocity = new Vector2(9, 0).RotatedBy(entity.Rotation - MathHelper.PiOver2);
				}
				if (Timer > 100)
				{
					float accelerationY = (accelerationYCoefficient * accelerationYCoefficient * accelerationYCoefficient - accelerationYCoefficient * accelerationYCoefficient * entity.ai[0]) / 300f;
					if (accelerationY > 0.03f)
					{
						accelerationY = 0.03f;
					}
					entity.Velocity.Y += accelerationY;
					entity.Velocity *= 0.99f;
					if (Timer >= 300 && Timer <= 380)
					{
						entity.Velocity *= 0.95f;
					}
					float speedValueMax = MathF.Pow(entity.ai[1], 2.4f);
					if (entity.Velocity.Y > speedValueMax && accelerationY > 0)
					{
						entity.Velocity.Y *= speedValueMax / entity.Velocity.Y;
					}
					if (entity.Scale < entity.ai[1] * 2 && entity.TimeLeft > 100)
					{
						entity.Scale += 0.005f;
					}
					if (entity.TimeLeft < 100)
					{
						entity.Scale *= 0.96f;
					}

					if (entity.Scale < 0.05f)
					{
						entity.Active = false;
					}
				}
				if (Timer > 0 && Timer <= 120)
				{
					entity.Alpha = Timer / 120f;
				}
				if (Timer > 120 && Timer < 380)
				{
					entity.Rotation = entity.Velocity.ToRotation() + MathHelper.PiOver2;
				}
				if (Timer >= 380)
				{
					entity.Rotation = 0;
				}
				entity.OldPositions.Enqueue(entity.Position);
				if (entity.OldPositions.Count > 30)
				{
					entity.OldPositions.Dequeue();
				}
				entity.Position += entity.Velocity;
				entity.ai[2] += entity.Velocity.Length() * 0.01f;
				entity.TimeLeft--;
				if (entity.TimeLeft <= 0)
				{
					entity.Active = false;
				}
			}
			ProjEntities[i] = entity;
		}
	}

	public override void AI()
	{
		UpdateEntity();
		Timer++;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var ent in ProjEntities)
		{
			if (ent.Active)
			{
				int size = (int)(ent.Scale * 10);
				Rectangle collisionBox = new Rectangle(-size + (int)ent.Position.X, -size + (int)ent.Position.Y, size, size);
				if (collisionBox.Intersects(targetHitbox))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		var frameBody = new Rectangle(0, 0, 20, 20);
		var frameBound = new Rectangle(0, 20, 20, 20);
		if (ProjEntities.Count > 0)
		{
			for (int i = 0; i < ProjEntities.Count; i++)
			{
				var ent = ProjEntities[i];
				if (ent.Active)
				{
					float fade = 1f;
					if (ent.TimeLeft <= 30)
					{
						fade = ent.TimeLeft / 30f;
					}

					float sinT = (MathF.Sin((float)Main.time * 0.08f + ent.ai[0] * 45) + 2) / 3f;
					var colorT = new Color(sinT, sinT, sinT, 0.5f * sinT) * ent.Alpha;

					Main.EntitySpriteDraw(tex, ent.Position - Main.screenPosition, frameBody, colorT * fade, ent.Rotation, frameBody.Size() * 0.5f, ent.Scale, SpriteEffects.None, 0);
					Main.spriteBatch.Draw(ModAsset.LanternFire.Value, ent.Position - Main.screenPosition, new Rectangle?(new Rectangle(0, 30 * (int)((ent.TimeLeft * 0.1) % 3), 20, 30)), colorT, 0, new Vector2(10, 15), ent.Scale * 0.5f, SpriteEffects.None, 1f);

					float distanceValue = (ent.Position - Main.LocalPlayer.Center).Length();
					float threadthodDis = 150;
					if (distanceValue < threadthodDis)
					{
						distanceValue = 1f;
					}
					if (distanceValue >= threadthodDis && distanceValue < threadthodDis * 2)
					{
						distanceValue = (threadthodDis * 2 - distanceValue) / threadthodDis;
					}
					if (distanceValue > threadthodDis * 2)
					{
						distanceValue = 0;
					}
					var boundColor = Color.Lerp(Color.Red, new Color(1f, 0.7f, 0.3f, 1f), distanceValue) * distanceValue;
					Main.spriteBatch.Draw(tex, ent.Position - Main.screenPosition, frameBound, boundColor * ent.Alpha * fade, ent.Rotation, frameBound.Size() * 0.5f, ent.Scale, SpriteEffects.None, 1f);
				}
			}
			if (Timer >= 100 && Timer < 380)
			{
				DrawTrail();
			}
		}
		return false;
	}

	public void DrawTrail()
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		float trailLength = 30;
		for (int i = 0; i < ProjEntities.Count; i++)
		{
			var ent = ProjEntities[i];
			if (ent.Active)
			{
				var bars0 = new List<Vertex2D>();
				var arrayPos = ent.OldPositions.ToArray();
				for (int j = 1; j < arrayPos.Count(); ++j)
				{
					// factor, among 0 to 1, usually for deciding the trail's texture.coord.X.
					float mulFac = Timer / trailLength;
					if (mulFac > 1f)
					{
						mulFac = 1f;
					}
					float factor = (j + 1) / (float)arrayPos.Count() * mulFac;

					float width = 0;
					if (j >= 0)
					{
						width = TrailWidthFunction(factor);
					}

					// timeValue, animate the trail.
					float timeValue = ent.ai[2] + ent.ai[0] * 15;
					Vector2 drawPos = ent.Position;
					if (j >= 0)
					{
						drawPos = arrayPos[j];
					}
					Color drawColor = GetTrailColor(ent, j);
					Vector2 dir = Vector2.zeroVector;
					if (j > 0)
					{
						dir = arrayPos[j] - arrayPos[j - 1];
					}
					else
					{
						dir = arrayPos[j] - ent.Position;
					}
					if (j <= 131 - Timer)
					{
						drawColor *= 0;
					}
					dir = dir.NormalizeSafe().RotatedBy(MathHelper.PiOver2);
					bars0.Add(drawPos + dir * 20, drawColor, ModifyTrailTextureCoordinate(factor, timeValue, 0, width));
					bars0.Add(drawPos - dir * 20, drawColor, ModifyTrailTextureCoordinate(factor, timeValue, 1, width));
				}
				if (bars0.Count > 2)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars0.ToArray(), 0, bars0.Count - 2);
				}
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public float TrailWidthFunction(float factor)
	{
		if (factor < 0)
		{
			return 0;
		}
		factor = 1 - factor;
		factor = MathF.Pow(factor, 0.5f);
		return MathF.Sin(factor * MathHelper.Pi);
	}

	public Color GetTrailColor(MovingEntity ent, int index)
	{
		float fade = 1f;
		if (Timer > 360)
		{
			fade = 0;
		}
		float value = index / (float)ent.OldPositions.Count;
		return Color.Lerp(new Color(1f, 0.7f, 0f, 0f), new Color(0.5f, 0, 0.1f, 0), 1 - value) * fade;
	}

	public Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor * 1f + timeValue * 0.5f;
		float y = 1;
		float z = widthValue;
		if (phase % 2 == 1)
		{
			y = 0f;
		}
		return new Vector3(x, y, z);
	}
}
