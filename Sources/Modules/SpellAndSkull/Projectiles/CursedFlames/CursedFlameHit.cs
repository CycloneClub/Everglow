using Everglow.Commons.MEAC;
using Everglow.Commons.VFX;
using Everglow.SpellAndSkull.Common;

namespace Everglow.SpellAndSkull.Projectiles.CursedFlames;

public class CursedFlameHit : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void AI()
	{
		if (Projectile.timeLeft <= 198)
		{
			Projectile.friendly = false;
		}

		Projectile.velocity *= 0;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 4 * Projectile.ai[0];
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 4 * Projectile.ai[0];
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 4 * Projectile.ai[0];
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 4 * Projectile.ai[0];
		return bool0 || bool1 || bool2 || bool3;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(55, 255, 0, 0) * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = ModAsset.CursedHitStar.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(55, 255, 0, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] / 40f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(55, 255, 0, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] / 40f, SpriteEffects.None, 0);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;
		float width = 60;
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft;
		}

		SpellAndSkullUtils.DrawTexCircle_Warp(spriteBatch, value * 27 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Projectile.Center.X > target.Center.X)
		{
			modifiers.HitDirectionOverride = -1;
		}
		if (Projectile.Center.X < target.Center.X)
		{
			modifiers.HitDirectionOverride = 1;
		}
		if (Projectile.Center.X == target.Center.X)
		{
			modifiers.HitDirectionOverride = 0;
		}
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.CursedInferno, 300);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(BuffID.CursedInferno, 300);
	}
}
