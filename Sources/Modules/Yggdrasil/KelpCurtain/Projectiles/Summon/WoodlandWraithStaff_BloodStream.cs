using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class WoodlandWraithStaff_BloodStream : TrailingProjectile
{
	public const float GlowTime = 90;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.4f, 0.0f, 0.0f, 0f);
		TrailWidth = 12f;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		TrailLength = 20;
		Projectile.width = 20;
		Projectile.height = 20;
	}

	public override void Behaviors()
	{
		Projectile.velocity.Y += 0.5f;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		bool sporeZone = false;
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active && proj.type == ModContent.ProjectileType<WoodlandWraithStaff_SporeZone>() && proj.owner == Projectile.owner)
			{
				WoodlandWraithStaff_SporeZone wWSSZ = proj.ModProjectile as WoodlandWraithStaff_SporeZone;
				if (Vector2.Distance(proj.Center, target.Center) < wWSSZ.Range)
				{
					sporeZone = true;
				}
			}
		}
		if (sporeZone)
		{
			modifiers.FinalDamage += WoodlandWraithStaff_FungiBall.DamangeBonusToTargetInSporeZone;
		}
	}

	public override void DestroyEntityEffect()
	{
		for (int k = 0; k < 6; k++)
		{
			float mulScale = Main.rand.NextFloat(6f, 20f);
			var blood = new BloodDrop
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(3, 6)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(82, 164),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var color = Color.Lerp(Color.White, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), MathF.Sqrt(Timer / GlowTime));
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, color, Projectile.velocity.ToRotation() - MathHelper.PiOver2, texMain.Size() / 2f, 0.6f, SpriteEffects.None, 0);
	}
}
