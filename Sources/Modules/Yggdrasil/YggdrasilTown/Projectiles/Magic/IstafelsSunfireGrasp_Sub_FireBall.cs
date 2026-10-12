using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.IstafelsEffects;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class IstafelsSunfireGrasp_Sub_FireBall : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public Vector2 SpawnPos;

	public Projectile MotherProj = default;

	public override string Texture => Commons.ModAsset.Point_Mod;

	public override void SetCustomDefaults()
	{
		TrailLength = 20;
		TrailColor = new Color(1, 1, 1, 0f);
		TrailBackgroundDarkness = 0.5f;
		TrailWidth = 20f;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		WarpStrength = 0.3f;
		SelfLuminous = false;
		Projectile.magic = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active)
			{
				if (proj.type == ModContent.ProjectileType<IstafelsSunfireGrasp_FireBall>())
				{
					if ((proj.Center - Projectile.Center).Length() < 30)
					{
						MotherProj = proj;
						break;
					}
				}
			}
		}
		if (MotherProj == default)
		{
			Projectile.active = false;
			return;
		}
		Projectile.hide = true;
		Timer = 0;
	}

	public override void Behaviors()
	{
		if (MotherProj == default || !MotherProj.active || MotherProj.type != ModContent.ProjectileType<IstafelsSunfireGrasp_FireBall>())
		{
			Projectile.active = false;
			return;
		}
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
		if (Timer < 60)
		{
			Projectile.Center = MotherProj.Center + (Projectile.velocity + MotherProj.velocity).NormalizeSafe() * MotherProj.width * 0.2f - Projectile.velocity;
			return;
		}
		if (Timer == 60)
		{
			for (int i = 0; i < 12; i++)
			{
				Vector2 afterVelocity = Projectile.velocity * Main.rand.NextFloat(0, i / 10f) + new Vector2(0, Main.rand.NextFloat(0, (12 - i) / 10f)).RotatedByRandom(MathHelper.TwoPi);
				float mulScale = Main.rand.NextFloat(4f, 8f);
				var drop = new IstafelsSunfireDrop
				{
					Velocity = afterVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(122, 204),
					Scale = mulScale,
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(drop);
			}
		}
		if (Timer > 60)
		{
			if (Main.rand.NextBool(12))
			{
				Vector2 afterVelocity = Projectile.velocity * Main.rand.NextFloat(0.4f, 1);
				float mulScale = Main.rand.NextFloat(4f, 8f);
				var drop = new IstafelsSunfireDrop
				{
					Velocity = afterVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(62, 124),
					Scale = mulScale,
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(drop);
			}
			var target = ProjectileUtils.FindTarget(Projectile.Center, 2000);
			if (target >= 0 && Collision.CanHit(Main.npc[target], Projectile))
			{
				var direction = (Main.npc[target].Center - Projectile.Center - Projectile.velocity).NormalizeSafe();
				Projectile.velocity = Projectile.velocity * 0.9f + direction * 0.1f * 12f;
			}
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override void DrawSelf()
	{
		if (Timer < 60)
		{
			float drawSize = Timer / 60f * 0.3f;
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			var drawColor = new Color(1f, 0.7f, 0.6f, 0f);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor, 0, star.Size() * 0.5f, drawSize, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, drawSize, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor * 0.5f, MathHelper.PiOver4, star.Size() * 0.5f, drawSize * 0.5f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor * 0.5f, -MathHelper.PiOver4, star.Size() * 0.5f, drawSize * 0.5f, SpriteEffects.None, 0);
		}
		else
		{
			Texture2D proj = Commons.ModAsset.EllipsesProj.Value;
			Main.spriteBatch.Draw(proj, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.7f, 0.1f, 0.8f), Projectile.rotation, proj.Size() * 0.5f, 0.2f, SpriteEffects.None, 0);
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			Color drawC;
			if (index < 6)
			{
				drawC = Color.Lerp(new Color(1f, 0.8f, 0.05f, 0), new Color(1f, 0.2f, 0f, 0), index / 6f);
			}
			else if (index is >= 6 and < 12)
			{
				drawC = Color.Lerp(new Color(1f, 0.2f, 0f, 0), new Color(0f, 0f, 0f, 0), (index - 6) / 6f);
			}
			else
			{
				drawC = new Color(0, 0, 0, 0);
			}
			return drawC;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override void DestroyEntityEffect()
	{
		for (int i = 0; i < 8; i++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(4f)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(4f, 8f);
			var drop = new IstafelsSunfireDrop
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(122, 204),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(drop);
		}
	}
}
