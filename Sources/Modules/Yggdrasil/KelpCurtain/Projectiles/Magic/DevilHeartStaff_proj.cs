using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class DevilHeartStaff_proj : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.85f, 0.75f, 0.65f, 0f);
		TrailWidth = 12f;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		TrailLength = 20;

		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 20;
		Projectile.height = 20;
		ProjTrailColor.colorList.Add((new Color(203, 73, 229, 0), 0));
		ProjTrailColor.colorList.Add((new Color(95, 99, 226, 0), 0.4f));
		ProjTrailColor.colorList.Add((new Color(206, 187, 165, 0), 0.8f));
		ProjTrailColor.colorList.Add((new Color(104, 104, 104, 0), 1f));
	}

	public GradientColor ProjTrailColor = new();

	public override void AI()
	{
		base.AI();
		if (Main.rand.NextBool(4))
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(0.6f, 1.4f)).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity;
			var dust = new DevilHeart_Spark
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(60, 90),
				Scale = Main.rand.NextFloat(3f, 5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		base.ModifyHitNPC(target, ref modifiers);
		target.AddBuff(BuffID.ChaosState, 120);
	}

	public override void DestroyEntityEffect()
	{
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 8.4f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = new DevilHeart_Spark
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(3f, 5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var color = new Color(225, 68, 223, 0);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, color, Projectile.velocity.ToRotation() - MathHelper.PiOver2, texMain.Size() / 2f, 0.6f, SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			return ProjTrailColor.GetColor(index / (float)SmoothedOldPos.Count);
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}
}
