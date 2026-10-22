using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class AncientPhantom_Magic_Explosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 16;
	}

	public override void AI()
	{
		var value = Projectile.timeLeft / 16f;
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 1.6f, 2f) * MathF.Pow(value, 2));
	}

	public void KillEffect()
	{
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		var value = Projectile.timeLeft / 16f;
		var drawColor = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(0f, 0f, 1f, 0), 1 - value);
		for (int t = 0; t < 3; t++)
		{
			Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation + t * MathHelper.TwoPi / 3f, star.Size() * 0.5f, new Vector2(value, 1), SpriteEffects.None, 0);
		}
		if (Projectile.timeLeft >= 15)
		{
			Texture2D pie = Commons.ModAsset.PieChartBloom.Value;
			Main.EntitySpriteDraw(pie, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, pie.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return MathUtils.IntersectsCircleAABB(Projectile.Center, 100, targetHitbox);
	}
}
