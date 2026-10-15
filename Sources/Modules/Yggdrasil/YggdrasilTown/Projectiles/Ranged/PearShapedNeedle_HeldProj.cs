using Everglow.Commons.Templates.Weapons;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class PearShapedNeedle_HeldProj : HandholdProjectile
{
	public float Timer = 0;

	public float MinChargeTime = 60;

	public float PrepareChargeAnimationPerNeedle = 3;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDef()
	{
		Projectile.friendly = false;
		DepartLength = 30;
	}

	public override void AI()
	{
		Timer++;
		float preparedTime = 13 * PrepareChargeAnimationPerNeedle;
		if (Timer % 3 == 0 && Timer >= 3 && Timer <= preparedTime)
		{
			SoundEngine.PlaySound(22, Projectile.Center);
		}
		base.AI();
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem.type != ModContent.ItemType<Items.Weapons.PearShapedNeedle>())
		{
			Projectile.Kill();
		}
		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);

		Vector2 mouseToPlayer = Main.MouseWorld - ArmRootPos;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;

		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);

		Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;
		if (player.controlUseItem)
		{
			Projectile.timeLeft = 10;
		}
		if (Projectile.Center.X < ArmRootPos.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawNeedles();
		return base.PreDraw(ref lightColor);
	}

	public void DrawNeedles()
	{
		for (int k = 0; k < 13; k++)
		{
			float distance = GetDistance(k);
			if (distance < 0)
			{
				break;
			}
			DrawNeedlePiece(k);
			if (k > 0)
			{
				DrawNeedlePiece(-k);
			}
		}
	}

	public float GetDistance(int k)
	{
		k = Math.Abs(k);
		float needlePhase = Timer - k * PrepareChargeAnimationPerNeedle;
		if (needlePhase < 0)
		{
			return -1;
		}
		float distance = needlePhase * 18;
		float maxDis = 30 + 5 * MathF.Sin(Timer * 0.1f + (k % 2) * 0.1f + k);
		if (k % 2 == 0)
		{
			maxDis += 10;
		}
		if (!NeedleReady(k))
		{
			maxDis *= 0.5f;
		}
		if (distance > maxDis)
		{
			distance = maxDis;
		}
		return distance;
	}

	public float GetShineValue(int k)
	{
		k = Math.Abs(k);
		float shineValue = 0;
		float preparedTime = 13 * PrepareChargeAnimationPerNeedle;
		if (Timer > preparedTime)
		{
			shineValue = (Timer - preparedTime) / (MinChargeTime - preparedTime) * 13f;
			shineValue = 3 - (k - shineValue);
			shineValue = Math.Max(shineValue, 0) / 3f;
		}
		if (shineValue > 1)
		{
			shineValue = MathF.Sin(Timer * 0.1f + (k % 2) * 0.1f + k) + 1;
			shineValue *= 0.1f;
		}
		return shineValue;
	}

	public void DrawNeedlePiece(int k)
	{
		Texture2D needle = ModAsset.PearShapedNeedle_Needle.Value;
		Texture2D needle_bloom = ModAsset.PearShapedNeedle_shine.Value;
		Texture2D needleReflect = ModAsset.PearShapedNeedle_Needle_reflection.Value;
		Texture2D needleReflectStar = ModAsset.PearShapedNeedle_shine_star.Value;
		float kAngle = 0.14f;
		Vector2 drawPos = GetNeedleWorldPos(k);
		Color lightColor = Lighting.GetColor(drawPos.ToTileCoordinates());
		drawPos -= Main.screenPosition;
		float rotation = Projectile.rotation + k * kAngle;
		Main.EntitySpriteDraw(needle, drawPos, null, lightColor, rotation, needle.Size() * 0.5f, 1f, SpriteEffects.None, 0);

		float shineValue = GetShineValue(k);
		if (shineValue > 0)
		{
			Color shineColor = lightColor;
			shineColor.A = 0;
			shineColor *= shineValue;
			Main.EntitySpriteDraw(needle_bloom, drawPos, null, shineColor, rotation, needle_bloom.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}

		float reflect = GetReflectValue(rotation);
		Color reflectColor = lightColor * reflect;
		reflectColor.A = 0;
		Main.EntitySpriteDraw(needleReflect, drawPos, null, reflectColor, rotation, needleReflect.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		if (reflect > 0.5f)
		{
			Vector2 addPos = new Vector2(7, -7).RotatedBy(rotation);
			Main.EntitySpriteDraw(needleReflectStar, drawPos + addPos, null, reflectColor * 2, 0, needleReflectStar.Size() * 0.5f, (reflect - 0.5f) * 3f, SpriteEffects.None, 0);
		}
	}

	public float GetReflectValue(float rotation)
	{
		float reflect = (MathF.Sin(rotation * 2 + MathHelper.PiOver4) + 1) * 0.5f;
		reflect = MathF.Pow(reflect, 2);
		return reflect;
	}

	public override void OnKill(int timeLeft)
	{
		ReleaseNeedles();
		base.OnKill(timeLeft);
	}

	public bool NeedleReady(int k)
	{
		k = Math.Abs(k);
		float shineValue = 0;
		float preparedTime = 13 * PrepareChargeAnimationPerNeedle;
		if (Timer > preparedTime)
		{
			shineValue = (Timer - preparedTime) / (MinChargeTime - preparedTime) * 13f;
			shineValue = 3 - (k - shineValue);
			shineValue = Math.Max(shineValue, 0) / 3f;
		}
		if (shineValue > 1)
		{
			return true;
		}
		return false;
	}

	public Vector2 GetNeedleWorldPos(int k)
	{
		return Projectile.Center + GetReleaseVel(k);
	}

	public Vector2 GetReleaseVel(int k)
	{
		float kAngle = 0.14f;
		return new Vector2(GetDistance(k), 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + k * kAngle);
	}

	public void ReleaseNeedles()
	{
		bool succeed = false;
		for (int k = 0; k < 13; k++)
		{
			if (NeedleReady(k))
			{
				succeed = true;
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), GetNeedleWorldPos(k), GetReleaseVel(k).NormalizeSafe(), ModContent.ProjectileType<PearShapedNeedle_Needle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				if (k > 0)
				{
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), GetNeedleWorldPos(-k), GetReleaseVel(-k).NormalizeSafe(), ModContent.ProjectileType<PearShapedNeedle_Needle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				}
			}
		}
		if (succeed)
		{
			SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);
		}
	}
}
