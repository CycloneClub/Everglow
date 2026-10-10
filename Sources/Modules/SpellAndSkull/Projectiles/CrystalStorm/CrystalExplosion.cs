using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.SpellAndSkull.Common;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.CrystalStorm;

public class CrystalExplosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

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
		Projectile.extraUpdates = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 200;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float maxDis = MathF.Sqrt(timeValue) * 24 * Projectile.ai[0];
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < maxDis;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < maxDis;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < maxDis;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < maxDis;
		return bool0 || bool1 || bool2 || bool3;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 100) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * (dark / 3f), 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 45f * dark, SpriteEffects.None, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 120) / 80f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = ModAsset.LineLight_2.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(135, 135, 255, 0), 1.57f, light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 0.2f, SpriteEffects.None, 0);
		return false;
	}

	private void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h / radius * 2, 0, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h / radius * 2, 1, 0)));
		}

		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
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

		SpellAndSkullUtils.DrawTexCircle_Warp(spriteBatch, MathF.Sqrt(value) * 36 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.06f * value,
			colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Ichor, 900);
	}
}
