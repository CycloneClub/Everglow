namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class CylindricalLantern_flame : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public struct MovingEntity()
	{
		public Vector2 Position;
		public Vector2 Velocity;
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
		Projectile.timeLeft = 500;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		ProjEntities = new List<MovingEntity>();
	}

	public float Timer = 0;

	public void UpdateEntity()
	{
		Vector2 targetPos = Vector2.zeroVector;
		int targetIndex = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
		Player player = null;
		if (targetIndex != -1)
		{
			player = Main.player[targetIndex];
		}
		if (player != null)
		{
			targetPos = player.Center;
		}
		int continueTimer = 120;
		if (Timer <= continueTimer)
		{
			if (Projectile.timeLeft % 60 == 0)
			{
				var entity = new MovingEntity();
				entity.Position = Projectile.Center;
				entity.Velocity = (targetPos - Projectile.Center).NormalizeSafe() * 4f;
				entity.TimeLeft = Main.rand.Next(120, 150);
				entity.Active = true;
				ProjEntities.Add(entity);
			}
		}
		for (int i = 0; i < ProjEntities.Count; i++)
		{
			MovingEntity entity = ProjEntities[i];
			if (entity.Active)
			{
				entity.Position += entity.Velocity;
				if (Collision.IsWorldPointSolid(entity.Position))
				{
					if (entity.TimeLeft > 30)
					{
						entity.TimeLeft = 30;
					}
					entity.Velocity *= 0;
				}
				entity.TimeLeft--;
				if (entity.TimeLeft <= 0)
				{
					entity.Active = false;
				}
				if (entity.TimeLeft <= 30)
				{
					entity.Velocity *= 0.95f;
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
				var collisionBox = new Rectangle(-6 + (int)ent.Position.X, -6 + (int)ent.Position.Y, 12, 12);
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
				int frameNumber = (int)((ent.TimeLeft + i * 4) / 7f) % 3;
				Rectangle frame = new Rectangle(0, 28 * frameNumber, 14, 28);
				if (ent.Active)
				{
					float fade = 1f;
					if (ent.TimeLeft <= 30)
					{
						fade = ent.TimeLeft / 30f;
					}
					Main.EntitySpriteDraw(tex, ent.Position - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0) * fade, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
					Lighting.AddLight(ent.Position, new Vector3(1f, 0.5f, 0.2f) * 0.6f * fade);
				}
			}
		}
		return false;
	}
}
