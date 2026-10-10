using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Guard_Attack_Spear : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public virtual float HoldoutRangeMin => 24f;

	public virtual float HoldoutRangeMax => 72f;

	public Vector2 LockCenter = Vector2.Zero;

	public Vector2 NPCCenter = Vector2.Zero;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.Spear);
		Projectile.friendly = true;
		Projectile.ownerHitCheck = false;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPCCenter = new Vector2(Projectile.Center.X, Projectile.Center.Y);
		base.OnSpawn(source);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreAI()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI / 2;
		int duration = 22;

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
		Projectile.Center = NPCCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}
