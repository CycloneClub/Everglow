namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class SmallLanternGroup_LanternRain : ModProjectile
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
		public float[] ai;
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
		Projectile.timeLeft = 700;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		ProjEntities = new List<MovingEntity>();
	}

	public float Timer = 0;

	public int ReleaseTimer = 0;

	public void UpdateEntity()
	{
		int continueTimer = 300;
		int releaseThrethod = 12;
		if (Timer > 30)
		{
			releaseThrethod = 8;
		}
		if (Timer > 60)
		{
			releaseThrethod = 6;
		}
		if (Timer > 80)
		{
			releaseThrethod = 4;
		}
		if (Timer > 630)
		{
			releaseThrethod = 6;
		}
		if (Timer > 660)
		{
			releaseThrethod = 10;
		}
		if (Timer < continueTimer)
		{
			ReleaseTimer++;
			switch (Projectile.ai[0])
			{
				case 0:
					if (ReleaseTimer >= releaseThrethod)
					{
						ReleaseTimer = 0;
						for (int i = 0; i < 12; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center + new Vector2(-2000 + i * 400, -1200);
							entity.Velocity = new Vector2(6, 6);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = 0;
							entity.TimeLeft = Main.rand.Next(350, 400);
							entity.Active = true;
							entity.ai = [0, 0];
							ProjEntities.Add(entity);
						}

						for (int i = 0; i < 12; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center + new Vector2(2000 - i * 400, -1200);
							entity.Velocity = new Vector2(-6, 6);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = 0;
							entity.TimeLeft = Main.rand.Next(350, 400);
							entity.Active = true;
							entity.ai = [0, 0];
							ProjEntities.Add(entity);
						}
					}
					break;
				case 1:
					if (ReleaseTimer >= releaseThrethod)
					{
						float timeValue = Timer;
						timeValue = timeValue / continueTimer;
						timeValue *= 24;
						ReleaseTimer = 0;
						for (int i = -16; i <= 16; i++)
						{
							MovingEntity entity = new MovingEntity();
							entity.Position = Projectile.Center + new Vector2(i * 240, -1200 + MathF.Sin(i) * 60);
							entity.Velocity = new Vector2(0, 6);
							entity.Scale = 1;
							entity.Rotation = 0;
							entity.RotationSpeed = 0;
							entity.TimeLeft = Main.rand.Next(350, 400);
							entity.Active = true;
							entity.ai = [i, 0];
							ProjEntities.Add(entity);
						}
					}
					break;
				case 2:
					if (Timer % 60 == 0)
					{
						// Notice that in this case, velocity is work for center.
						float offset = 0;
						if (Timer % 120 == 0)
						{
							offset = 300;
						}
						for (int j = -4; j <= 4; j++)
						{
							int range = (int)(25 + 4 * MathF.Sin(Timer + j));
							for (int i = 0; i < range; i++)
							{
								MovingEntity entity = new MovingEntity();
								entity.Position = Projectile.Center + new Vector2(j * 600 + offset, -1400);
								entity.Velocity = Projectile.Center + new Vector2(j * 600 + offset, -1400);
								entity.Scale = 1;
								entity.Rotation = 0;
								entity.RotationSpeed = 0;
								entity.TimeLeft = Main.rand.Next(350, 400);
								entity.Active = true;
								entity.ai = [i, 3, range];
								ProjEntities.Add(entity);
							}
						}
					}
					break;
				case 3:
					if (Timer % 30 == 0)
					{
						int deltaX = 0;
						if (Timer % 60 == 0)
						{
							deltaX = 150;
						}
						for (int i = -12; i <= 12; i++)
						{
							for (int j = 0; j <= 6; j++)
							{
								MovingEntity entity = new MovingEntity();
								entity.Position = Projectile.Center + new Vector2(i * 300 + deltaX, -1200 - j * 30);
								entity.Velocity = new Vector2(0, 6);
								entity.Scale = 1;
								entity.Rotation = 0;
								entity.RotationSpeed = 0;
								entity.TimeLeft = Main.rand.Next(350, 400);
								entity.Active = true;
								entity.ai = [i, 0];
								ProjEntities.Add(entity);
							}
						}
					}
					break;
				default:
					if (Timer % 30 == 0)
					{
						int deltaX = 0;
						if (Timer % 60 == 0)
						{
							deltaX = 150;
						}
						for (int i = -12; i <= 12; i++)
						{
							for (int j = 0; j <= 6; j++)
							{
								MovingEntity entity = new MovingEntity();
								entity.Position = Projectile.Center + new Vector2(i * 300 + deltaX, -1200 - j * 30);
								entity.Velocity = new Vector2(0, 6);
								entity.Scale = 1;
								entity.Rotation = 0;
								entity.RotationSpeed = 0;
								entity.TimeLeft = Main.rand.Next(350, 400);
								entity.Active = true;
								entity.ai = [i, 0];
								ProjEntities.Add(entity);
							}
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
				// ai[0] == 2 means it act as several descending wheels.
				if (Projectile.ai[0] != 2)
				{
					entity.Position += entity.Velocity;
					entity.Velocity = entity.Velocity.RotatedBy(entity.RotationSpeed);
				}
				else
				{
					entity.Velocity.Y += entity.ai[1];
					entity.ai[1] += 0.02f;
					int dir = 1;
					if (entity.ai[2] % 2 == 1)
					{
						dir = -1;
					}
					entity.Position = entity.Velocity + new Vector2(0, entity.ai[2] * 6).RotatedBy((entity.ai[0] / entity.ai[2] * MathHelper.TwoPi + Timer * 0.2f / entity.ai[2]) * dir);
				}
				if (Projectile.ai[0] == 1)
				{
					entity.Velocity.X = MathF.Sin(Timer * 0.02f + entity.Position.Y * 0.01f + entity.ai[0] * MathF.PI) * 4;
				}

				entity.RotationSpeed *= 0.96f;
				entity.Rotation = entity.Velocity.ToRotation() + MathHelper.PiOver2;
				if (Projectile.ai[0] == 2)
				{
					int dir = 1;
					if (entity.ai[2] % 2 == 1)
					{
						dir = -1;
					}
					entity.Rotation = (entity.ai[0] / entity.ai[2] * MathHelper.TwoPi + Timer * 0.2f / entity.ai[2]) * dir + MathHelper.PiOver2;
				}
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
		Projectile.velocity *= 0;
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
