using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class PylonStonePostProj_crimson : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = true;
		TrailColor = new Color(0.9f, 0f, 0f, 0);
		TrailBackgroundDarkness = 1;
		TrailTexture = Commons.ModAsset.Trail_3.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_3_black.Value;
		TrailWidth = 80;
		TrailLength = 60;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		if (Projectile.timeLeft >= 60)
		{
			Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
			Vector2 toPlayer = player.Center - Projectile.Center - Projectile.velocity;
			if (toPlayer.Length() > 80)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(toPlayer) * 5f, 0.05f + 0.01f * MathF.Sin((float)Main.time * 0.015f + Projectile.whoAmI) + Projectile.ai[0]);
			}
		}
		if (Projectile.timeLeft < 60)
		{
			DestroyEntity();
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			Color drawC = Color.Lerp(TrailColor, new Color(0.4f, 0f, 1f, 0), index / (float)SmoothedOldPos.Count);
			return drawC;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.5f;
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

	public override void DrawSelf()
	{
		var texDark = ModAsset.MothMiddleBullet_dark.Value;
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var texStar = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(texDark, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texDark.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.6f, 0.6f, 0), Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

		Vector2 addVel = Vector2.Normalize(Projectile.velocity) * 10;
		Main.spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + addVel, null, TrailColor, 0, texStar.Size() / 2f, 0.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + addVel, null, TrailColor, MathHelper.PiOver2, texStar.Size() / 2f, new Vector2(0.5f, 1f), SpriteEffects.None, 0);
	}

	public override void DestroyEntityEffect()
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<PylonStonePostProj_crimson_explosion>(), 50, 3, Projectile.owner, 10f);
	}
}
