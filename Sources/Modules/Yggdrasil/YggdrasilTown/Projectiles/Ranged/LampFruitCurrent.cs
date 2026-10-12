using Everglow.Commons.Templates.Weapons;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class LampFruitCurrent : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.ranged = true;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 3600;

		TrailBackgroundDarkness = 0;
		TrailTexture = Commons.ModAsset.Trail_16.Value;
		TrailLength = 10;
		TrailColor = new Color(1, 0.7f, 0.1f, 0f);
		TrailWidth = 15f;
	}

	public override bool PreAI()
	{
		if (TimeAfterEntityDestroy >= 0)
		{
			Lighting.AddLight(Projectile.Center, new Vector3(1, 0.7f, 0.1f) * TimeAfterEntityDestroy / 10f);
		}
		return true;
	}

	public override void Behaviors()
	{
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
		Lighting.AddLight(Projectile.Center, new Vector3(1, 0.7f, 0.1f));
		if (Main.rand.NextBool(5))
		{
			var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<Dusts.LampWood_Dust_fluorescent_appear>());
			dust.alpha = 0;
			dust.rotation = Main.rand.NextFloat(0.3f, 0.7f);
			dust.velocity = Projectile.velocity * 0.1f;
		}
	}

	public override void DrawSelf()
	{
		if (Projectile.timeLeft > 3597)
		{
			return;
		}
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color color = new Color(1f, 0.7f, 0.2f, 0);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texMain.Size() / 2f, 1f, SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			float value = index / (float)SmoothedOldPos.Count;
			Color color = Color.Lerp(new Color(1f, 0.7f, 0.2f, 0), new Color(0.2f, 0.05f, 1f, 0), value);
			return color;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor * 3 - timeValue * 1.5f;
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

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.rand.NextBool(2))
		{
			target.AddBuff(ModContent.BuffType<Buffs.Photolysis>(), 240);
		}
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void DestroyEntityEffect()
	{
		Vector2 unitVel = Projectile.velocity.NormalizeSafe();
		for (int i = 0; i < 30; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4) + unitVel * i * 2, 0, 0, ModContent.DustType<Dusts.LampWood_Dust_fluorescent_appear>());
			dust.alpha = 100;
			dust.rotation = Main.rand.NextFloat(0.3f, 0.9f);
			dust.velocity = unitVel * (i + 15) / 6f;
		}
	}
}
