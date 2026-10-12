using SteelSeries.GameSense;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class SphereLanternProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public struct MovingEntity()
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public float Scale;
		public float Rotation;
		public float RandomValue;
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

	public override void OnSpawn(IEntitySource source)
	{
		float f2 = 5;
		float randomValue = Main.rand.NextFloat(MathHelper.TwoPi);
		for (int i = 0; i < f2; i++)
		{
			MovingEntity entity = new MovingEntity();
			entity.Position = Projectile.Center;
			entity.Velocity = new Vector2(0, 3.6f).RotatedBy(i / f2 * MathHelper.TwoPi + randomValue);
			entity.Scale = 1;
			entity.Rotation = 0;
			entity.RandomValue = Main.rand.NextFloat(MathHelper.TwoPi);
			entity.TimeLeft = Main.rand.Next(350, 400);
			entity.Active = true;
			ProjEntities.Add(entity);
		}
	}

	public void UpdateEntity()
	{
		for (int i = 0; i < ProjEntities.Count; i++)
		{
			MovingEntity entity = ProjEntities[i];
			if (entity.Active)
			{
				entity.Position += entity.Velocity;
				entity.Velocity *= 1f;
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
		Texture2D glow = ModAsset.SphereLanternProj_glow.Value;
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
					float lightValue = MathF.Sin(ent.RandomValue + (float)Main.time * 0.1f) + 1;
					lightValue *= 0.5f;
					Main.EntitySpriteDraw(glow, ent.Position - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * lightValue * fade, ent.Rotation, glow.Size() * 0.5f, ent.Scale, SpriteEffects.None, 0);
				}
			}
		}
		return false;
	}
}
