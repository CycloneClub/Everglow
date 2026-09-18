using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.Legacies;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;

public class AncientSyringe_proj : HandholdProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDef()
	{
		DepartLength = 110;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		base.SetDef();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		player.itemTime = timeMax;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int times = 10;
		for (int x = 0; x < times; x++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(5f, 8f)).RotatedByRandom(6.283f);
			var splash = new LichenSlimeSplash
			{
				velocity = afterVelocity,
				Active = true,
				Visible = true,
				position = target.Center,
				maxTime = Main.rand.Next(12, 28),
				scale = Main.rand.NextFloat(6f, 18f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(splash);
		}
		for (int x = 0; x < times * 2; x++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(5f, 8f)).RotatedByRandom(6.283f) * Main.rand.NextFloat(6f, 15f);
			float mulScale = Main.rand.NextFloat(6f, 15f);
			var blood = new LichenSlimeDrop
			{
				velocity = afterVelocity / mulScale,
				Active = true,
				Visible = true,
				position = target.Center,
				maxTime = Main.rand.Next(32, 64),
				scale = mulScale,
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		target.AddBuff(ModContent.BuffType<LichenInfected>(), 480);
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void AI()
	{
		base.AI();
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem.type != ModContent.ItemType<AncientSyringe>())
		{
			Projectile.Kill();
		}
		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);

		Vector2 mouseToPlayer = Main.MouseWorld - ArmRootPos;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		if (Projectile.localNPCHitCooldown > timeMax - 1)
		{
			Projectile.localNPCHitCooldown = timeMax - 1;
		}
		if (player.controlUseItem)
		{
			if (player.itemTime == 1)
			{
				Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
				player.itemTime = timeMax;
			}
			float duration = player.itemTime / (float)timeMax;
			if (duration > 0.75f)
			{
				Projectile.friendly = true;
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
			}
			else if (duration > 0.5f)
			{
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
			}
			else if (duration > 0.25f)
			{
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
			}
			else
			{
				Projectile.friendly = false;
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
			}
			DepartLength = 110 - MathF.Pow(duration, 3.2f) * 90;

			Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;
			Projectile.timeLeft = timeMax;
		}
		if (!player.controlUseItem)
		{
			Projectile.Kill();
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
		Player player = Main.player[Projectile.owner];
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + DrawOffset - new Vector2(54, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, lightColor, rot, texMain.Size() / 2f, 1f, se, 0);
		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		float duration = player.itemTime / (float)timeMax;
		duration *= 1.5f;
		duration -= 0.5f;
		if (duration < 0)
		{
			duration = 0;
		}
		duration = MathF.Sin(duration * MathHelper.Pi);
		Texture2D star = Commons.ModAsset.StarSlash_black.Value;
		Color drawC = new Color(0.7f * lightColor.R / 255f, 2f * lightColor.G / 255f, 0.2f * lightColor.B / 255f, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + DrawOffset + new Vector2(24, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, Color.White, MathHelper.PiOver2, star.Size() / 2f, 0.5f * duration, se, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + DrawOffset + new Vector2(24, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, Color.White, 0, star.Size() / 2f, 0.5f * duration, se, 0);
		star = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + DrawOffset + new Vector2(24, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, drawC, MathHelper.PiOver2, star.Size() / 2f, 0.5f * duration, se, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + DrawOffset + new Vector2(24, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, drawC, 0, star.Size() / 2f, 0.5f * duration, se, 0);
		return false;
	}
}
