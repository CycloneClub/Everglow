using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.Items.Weapons;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class GildingRevolver_Hold : HandholdProjectile
{
	public int Timer = 0;

	public float NormalBulletTimer = 0;

	public int ReloadCooling = 0;

	public int NormalBulletsCount = 0;

	public int LanternBulletCount = 0;

	public int UsedBulletsCount = 0;

	public int LanternBulletTimer = 0;

	public int LanternBulletCooling = 0;

	public int LanternBulletCoolingMax = 600;

	public bool MouseInLanternZone = false;

	public override void SetDef()
	{
		Projectile.width = 64;
		Projectile.height = 64;
		Projectile.friendly = false;
		TextureRotation = 0;
		MaxRotationSpeed = 0.05f;
		DepartLength = 20;
		DrawOffset = new Vector2(0, -4);
		ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		if (ReloadCooling <= 0)
		{
			if (NormalBulletTimer < 120)
			{
				NormalBulletTimer += 6 * (5f / Main.LocalPlayer.itemTimeMax);
				//if(NormalBulletTimer % 20 < 6 * (5f / Main.LocalPlayer.itemTimeMax))
				//{
				//	SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_bullet_Mod), Projectile.Center);
				//}
				if(NormalBulletTimer >= 120)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_reloadcomplete_1_Mod), Projectile.Center);
							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_reloadcomplete_2_Mod), Projectile.Center);
							break;
					}
				}
			}

			if (NormalBulletTimer > 120)
			{
				NormalBulletTimer = 120;
			}
		}
		else
		{
			ReloadCooling--;
			if(ReloadCooling == 0)
			{
				switch (Main.rand.Next(2))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_reloading_1_Mod), Projectile.Center);
						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_reloading_2_Mod), Projectile.Center);
						break;
				}
			}
		}
		if (LanternBulletCooling <= 0)
		{
			if (LanternBulletTimer <= 6)
			{
				LanternBulletTimer++;
			}
			else
			{
				LanternBulletCount = 1;
			}
		}
		else
		{
			LanternBulletCount = 0;
			LanternBulletTimer = 0;
			LanternBulletCooling--;
		}
		NormalBulletsCount = (int)(NormalBulletTimer / 20f);
		Timer++;
		MouseInLanternZone = false;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<LanternZone>() && proj.owner == Projectile.owner)
			{
				if ((Main.MouseWorld - proj.Center).Length() < 70)
				{
					MouseInLanternZone = true;
					break;
				}
			}
		}
		base.AI();
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		int dir = 1;
		if (Main.MouseWorld.X < player.MountedCenter.X)
		{
			dir = -1;
		}

		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);
		Vector2 mouseToPlayer = Main.MouseWorld - ArmRootPos;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
		Projectile.Center = player.MountedCenter + mouseToPlayer * 12;
		Item item = player.HeldItem;
		if (item is not null)
		{
			GildingRevolver gildingRevolver = item.ModItem as GildingRevolver;
			if (gildingRevolver is not null)
			{
				Projectile.timeLeft = 2;
			}
		}

		if (player.controlUseItem && player.altFunctionUse == 0)
		{
			player.direction = dir;
			Projectile.hide = false;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
			if (MouseInLanternZone && player.itemTime == player.itemTimeMax)
			{
				ShootPhantom();
			}
			else if (player.itemTime == player.itemTimeMax
				&& NormalBulletTimer >= 20/* At least one bullet */)
			{
				Shoot();
				NormalBulletTimer -= 20;
				ReloadCooling = 25;
			}
		}
		else if (player.controlUseItem && player.altFunctionUse == 2)
		{
			player.direction = dir;
			Projectile.hide = false;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
			if (player.itemTime == player.itemTimeMax && LanternBulletCount >= 1)
			{
				Shoot_RightClick();
				LanternBulletCooling = LanternBulletCoolingMax;
			}
		}
		else
		{
			Projectile.hide = true;
		}
	}

	private void Shoot()
	{
		Player player = Main.player[Projectile.owner];
		var item = player.HeldItem;
		if (item is not null
			&& item.type == ModContent.ItemType<GildingRevolver>()
			&& item.ModItem is GildingRevolver gildingRevolver
			&& gildingRevolver.ShootType >= 0)
		{
			switch (Main.rand.Next(3))
			{
				case 0:
					SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_shoot_1_Mod).WithVolume(0.5f), Projectile.Center);
					break;
				case 1:
					SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_shoot_2_Mod).WithVolume(0.5f), Projectile.Center);
					break;
				case 2:
					SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_shoot_3_Mod).WithVolume(0.5f), Projectile.Center);
					break;
			}

			Vector2 vel = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2().RotatedByRandom(0.02f) * item.shootSpeed;
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + vel.NormalizeSafe() * -2 + DrawOffset, vel, gildingRevolver.ShootType, item.damage, item.knockBack, Projectile.owner);
			UsedBulletsCount++;
			if (NormalBulletsCount == 6 || NormalBulletsCount == 3)
			{
				vel = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2().RotatedByRandom(0.02f) * item.shootSpeed / 4f;
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + vel.NormalizeSafe() * -2 + DrawOffset, vel, ModContent.ProjectileType<LanternFlameBullet>(), item.damage, item.knockBack, Projectile.owner);
			}
		}
	}

	private void ShootPhantom()
	{
		SoundEngine.PlaySound(SoundID.Item101);
		Player player = Main.player[Projectile.owner];
		Item item = player.HeldItem;
		if (item is null)
		{
			return;
		}
		switch (Main.rand.Next(3))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_aperture_hit_1_Mod), Projectile.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_aperture_hit_2_Mod), Projectile.Center);
				break;
			case 2:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_aperture_hit_3_Mod), Projectile.Center);
				break;
		}
		Vector2 vel = (Main.MouseWorld - Projectile.Center).NormalizeSafe().RotatedByRandom(0.03f) * item.shootSpeed * 1.5f;
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + vel.NormalizeSafe() * -2 + DrawOffset, vel, ModContent.ProjectileType<LanternPhantomBullet>(), (int)(item.damage * 0.7f), item.knockBack, Projectile.owner);
	}

	private void Shoot_RightClick()
	{
		Player player = Main.player[Projectile.owner];
		Item item = player.HeldItem;
		if (item is null)
		{
			return;
		}
		Vector2 vel = (Main.MouseWorld - Projectile.Center).NormalizeSafe() * item.shootSpeed * 1.5f;
		switch (Main.rand.Next(3))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_aperture_hit_1_Mod), Projectile.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_aperture_hit_2_Mod), Projectile.Center);
				break;
			case 2:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_aperture_hit_3_Mod), Projectile.Center);
				break;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + vel.NormalizeSafe() * -2 + DrawOffset, vel, ModContent.ProjectileType<LanternZoneBullet>(), item.damage, item.knockBack, Projectile.owner);
		UsedBulletsCount++;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawBaseTexture(lightColor);
		return false;
	}

	public override void DrawBaseTexture(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		SpriteEffects sprite = SpriteEffects.None;
		if (player.gravDir == -1)
		{
			sprite = SpriteEffects.FlipVertically;
		}
		if (!Projectile.hide)
		{
			var texMain = ModAsset.GildingRevolver_Hold.Value;
			SpriteEffects se = SpriteEffects.None;
			if (player.direction == -1)
			{
				se = SpriteEffects.FlipVertically;
			}
			float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
			Vector2 drawCenter = Projectile.Center - Main.screenPosition + DrawOffset;
			Main.spriteBatch.Draw(texMain, drawCenter, null, lightColor, rot, texMain.Size() * 0.5f, 1f, se, 0);
		}

		var textureUI = ModAsset.GildingRevolver_Hold_Bullets.Value;
		Rectangle darkFrame = new Rectangle(0, 0, 92, 30);
		Main.spriteBatch.Draw(textureUI, player.Center - Main.screenPosition + new Vector2(0, -50 * player.gravDir), darkFrame, Color.White, 0, darkFrame.Size() * 0.5f, 1f, sprite, 0);
		if (!MouseInLanternZone)
		{
			for (int k = 0; k < 6; k++)
			{
				if (NormalBulletTimer < (k + 1) * 20 - 5)
				{
					break;
				}
				Rectangle normalFrame = new Rectangle(0, 32, 10, 26);
				if (k % 3 == 2)
				{
					normalFrame = new Rectangle(12, 32, 10, 26);
				}
				if (NormalBulletTimer < (k + 1) * 20)
				{
					normalFrame = new Rectangle(40, 32, 10, 26);
				}
				Main.spriteBatch.Draw(textureUI, player.Center - Main.screenPosition + new Vector2(k * 12 - 41, -50 * player.gravDir), normalFrame, Color.White, 0, normalFrame.Size() * 0.5f, 1f, sprite, 0);
			}
		}
		else
		{
			for (int k = 0; k < 6; k++)
			{
				Rectangle normalFrame = new Rectangle(68, 32, 10, 26);
				Main.spriteBatch.Draw(textureUI, player.Center - Main.screenPosition + new Vector2(k * 12 - 41, -50 * player.gravDir), normalFrame, Color.White, 0, normalFrame.Size() * 0.5f, 1f, sprite, 0);
			}
		}
		if (LanternBulletTimer > 2)
		{
			Rectangle lanternbulletFrame = new Rectangle(52, 30, 14, 30);
			if (LanternBulletTimer >= 6)
			{
				lanternbulletFrame = new Rectangle(24, 30, 14, 30);
			}
			Main.spriteBatch.Draw(textureUI, player.Center - Main.screenPosition + new Vector2(39, -49 * player.gravDir), lanternbulletFrame, Color.White, 0, lanternbulletFrame.Size() * 0.5f, 1f, sprite, 0);
		}
		else if (LanternBulletCooling > 0)
		{
			Rectangle lanternbulletFrameCooling = new Rectangle(52, 30, 14, 30);
			int duration = (int)(30f * LanternBulletCooling / LanternBulletCoolingMax);
			Rectangle lanternbulletFrameCooling_Draw = new Rectangle(52, 60 - duration, 14, duration);
			Main.spriteBatch.Draw(textureUI, player.Center - Main.screenPosition + new Vector2(39, (-50 + 30 - duration) * player.gravDir), lanternbulletFrameCooling_Draw, Color.White * 0.5f, 0, lanternbulletFrameCooling.Size() * 0.5f, 1f, sprite, 0);
		}
	}
}
