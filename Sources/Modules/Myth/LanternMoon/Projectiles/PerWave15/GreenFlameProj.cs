using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.VFX;
using Terraria;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class GreenFlameProj : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public NPC OwnerNPC;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0, 0.7f, 1, 0f);
		TrailBackgroundDarkness = 0.1f;
		TrailWidth = 16f;
		SelfLuminous = true;
		TrailLength = 24;
		TrailTexture = Commons.ModAsset.Trail_10.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
		Projectile.timeLeft = 300;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.hostile = true;
	}

	public override void Behaviors()
	{
		float minDis = 120;
		NPC targetNPC = null;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (npc.ModNPC is LanternMoonNPC && npc != OwnerNPC && npc.life < npc.lifeMax)
				{
					Vector2 toTarget = npc.Center - Projectile.Center;
					if (toTarget.Length() < minDis)
					{
						minDis = toTarget.Length();
						targetNPC = npc;
					}
				}
			}
		}
		if (targetNPC is null)
		{
			Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin((float)Main.time * 0.3f) * 0.3f);
		}
		else
		{
			Vector2 toTarget = targetNPC.Center - Projectile.Center - Projectile.velocity;
			if (toTarget.Length() < 20 && TimeAfterEntityDestroy < 0)
			{
				int healAmount = Main.rand.Next(75, 125);
				targetNPC.life += healAmount;
				if (targetNPC.life > targetNPC.lifeMax)
				{
					targetNPC.life = targetNPC.lifeMax;
				}
				targetNPC.HealEffect(healAmount);
				DestroyEntity();
			}
			toTarget = toTarget.NormalizeSafe() * 4f;
			Projectile.velocity = Projectile.velocity * 0.9f + toTarget * 0.1f;
		}
		if (Projectile.timeLeft > 60)
		{
			Vector2 newVelocity = Projectile.velocity * 0.5f;
			var sparkFlame = new GreenLanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-4, 4), 0),
				RotateSpeed = Main.rand.NextFloat(-1.2f, 1.2f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (Main.rand.Next(2) - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(45, 60),
				Scale = Main.rand.NextFloat(0.7f, 1.2f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}
	}

	public override void DestroyEntityEffect()
	{
		for (int i = 0; i < 34; i++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2.75f, 3.2f)).RotatedByRandom(MathHelper.TwoPi);
			var sparkFlame = new GreenLanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-4, 4), 0),
				RotateSpeed = Main.rand.NextFloat(-1.2f, 1.2f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (Main.rand.Next(2) - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(45, 60),
				Scale = Main.rand.NextFloat(0.3f, 1.2f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}
	}

	public override void DrawSelf()
	{
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color color = TrailColor;
		if (!SelfLuminous)
		{
			color = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		}
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texMain.Size() / 2f, fade, SpriteEffects.None, 0);
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float star_scale = 1f;
		float timeValue = MathF.Sin((float)Main.time * 0.07f + Projectile.whoAmI) * 0.5f + 0.5f;
		Color c0 = new Color(0f, 0.75f * timeValue, 0.75f, 0) * fade;
		var drawPos = Projectile.Center - Main.screenPosition;
		Main.spriteBatch.Draw(star, drawPos, null, c0, MathHelper.PiOver2, star.Size() / 2f, new Vector2(star_scale / 1.5f, 0.5f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, drawPos, null, c0, 0, star.Size() / 2f, new Vector2(star_scale / 1.5f, 0.5f), SpriteEffects.None, 0);

		Main.spriteBatch.Draw(star, drawPos, null, c0, MathHelper.PiOver4, star.Size() / 2f, star_scale / 5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, drawPos, null, c0, -MathHelper.PiOver4, star.Size() / 2f, star_scale / 5f, SpriteEffects.None, 0);

		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(spot, drawPos, null, new Color(1f, 1f, 0.7f, 0), 0, spot.Size() / 2f, star_scale * fade, SpriteEffects.None, 0);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style is 0 or 1)
		{
			float fade = 1f;
			if (Projectile.timeLeft < 60f)
			{
				fade *= Projectile.timeLeft / 60f;
			}
			return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1) * fade;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}
}
