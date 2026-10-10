namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class SmallLanternGroup : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public struct MovingEntity()
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public float Scale;
		public float Rotation;
		public float RotationSpeed;
		public int TimeLeft;
		public bool Active;
	}

	public List<MovingEntity> ProjEntities = new List<MovingEntity>();

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 900;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		ProjEntities = new List<MovingEntity>();
	}

	public float Timer = 0;

	public void UpdateEntity()
	{
		float frequency = 3;
		if (Main.expertMode)
		{
			frequency = 4;
		}
		if (Main.masterMode)
		{
			frequency = 5;
		}
		int continueTimer = 250;
		if (Timer < continueTimer)
		{
			switch (Projectile.ai[0])
			{
				case 0:
					if (Projectile.timeLeft % 4 == 0)
					{
						float timeValue = Timer;
						timeValue = timeValue / continueTimer;
						timeValue = MathF.Pow(timeValue, 2) * 3;
						for (int i = 0; i < frequency; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center;
							entity.Velocity = new Vector2(0, 4).RotatedBy(i / frequency * MathHelper.TwoPi + timeValue);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = MathF.Sin(timeValue / frequency * 5 * MathHelper.TwoPi) * 0.01f;
							entity.TimeLeft = Main.rand.Next(350, 400);
							entity.Active = true;
							ProjEntities.Add(entity);
						}
					}
					break;
				case 1:
					if (Projectile.timeLeft % 12 == 0)
					{
						float timeValue = Timer;
						timeValue = timeValue / continueTimer;
						timeValue = MathF.Pow(timeValue, 2) * 3;
						for (int i = 0; i < frequency; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center;
							entity.Velocity = new Vector2(0, 2.4f).RotatedBy(i / frequency * MathHelper.TwoPi + timeValue);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = MathF.Sin(timeValue * 0.6f) * 0.1f;
							entity.TimeLeft = Main.rand.Next(450, 500);
							entity.Active = true;
							ProjEntities.Add(entity);
						}
						for (int i = 0; i < frequency; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center;
							entity.Velocity = new Vector2(0, 2.4f).RotatedBy(i / frequency * MathHelper.TwoPi - timeValue);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = -MathF.Sin(timeValue * 0.6f) * 0.1f;
							entity.TimeLeft = Main.rand.Next(450, 500);
							entity.Active = true;
							ProjEntities.Add(entity);
						}
					}
					break;
				case 2:
					if (Timer % 30 == 0)
					{
						float timeValue = Timer;
						timeValue = timeValue / continueTimer;
						timeValue = MathF.Pow(timeValue, 2) * 3;
						float f2 = frequency * 6;
						for (int i = 0; i < f2; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center;
							entity.Velocity = new Vector2(0, 3.6f).RotatedBy(i / f2 * MathHelper.TwoPi + timeValue);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = (Timer % 60 == 0 ? -1 : 1) * 0.15f;
							entity.TimeLeft = Main.rand.Next(350, 400);
							entity.Active = true;
							ProjEntities.Add(entity);
						}
					}
					break;
				case 3:
					if (Timer % 75 == 0)
					{
						float timeValue = Timer;
						timeValue = timeValue / continueTimer;
						timeValue = MathF.Pow(timeValue, 2) * 3;
						float f2 = frequency * 2;
						for (int t = 0; t < 4; t++)
						{
							for (int i = 0; i < f2; i++)
							{
								Vector2 baseVel = new Vector2((i - f2 / 2f) / f2, -0.5f).RotatedBy(t / 4f * MathHelper.TwoPi) * 4f;
								MovingEntity entity = new MovingEntity();
								entity.Position = Projectile.Center;
								entity.Velocity = baseVel;
								entity.Scale = 1;
								entity.Rotation = 0;
								entity.RotationSpeed = timeValue * 0.3f;
								entity.TimeLeft = Main.rand.Next(450, 500);
								entity.Active = true;
								ProjEntities.Add(entity);
							}
						}
					}
					break;
				default:
					if (Projectile.timeLeft % 4 == 0)
					{
						float timeValue = Timer;
						timeValue = timeValue / continueTimer;
						timeValue = MathF.Pow(timeValue, 2) * 3;
						for (int i = 0; i < frequency; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center;
							entity.Velocity = new Vector2(0, 4).RotatedBy(i / frequency * MathHelper.TwoPi + timeValue);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = MathF.Sin(timeValue / frequency * 5 * MathHelper.TwoPi) * 0.01f;
							entity.TimeLeft = Main.rand.Next(350, 400);
							entity.Active = true;
							ProjEntities.Add(entity);
						}
					}
					break;
			}
		}
		for (int i = 0; i < ProjEntities.Count; i++)
		{
			MovingEntity entity = ProjEntities[i];
			if (entity.Active)
			{
				entity.Position += entity.Velocity;
				entity.Velocity = entity.Velocity.RotatedBy(entity.RotationSpeed);
				entity.Velocity *= 1f;
				entity.RotationSpeed *= 0.96f;
				entity.Rotation = entity.Velocity.ToRotation() + MathHelper.PiOver2;
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
		Timer++;
		UpdateEntity();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var ent in ProjEntities)
		{
			if (ent.Active)
			{
				Rectangle collisionBox = new Rectangle(-10 + (int)ent.Position.X, -10 + (int)ent.Position.Y, 20, 20);
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
					Main.EntitySpriteDraw(tex, ent.Position - Main.screenPosition, null, Color.White * fade, ent.Rotation, tex.Size() * 0.5f, ent.Scale, SpriteEffects.None, 0);
				}
			}
		}
		return false;
	}
}
