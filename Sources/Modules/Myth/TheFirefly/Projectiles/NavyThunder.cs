using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Audio;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class NavyThunder : ModProjectile, IWarpProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/NavyThunderTex/FlameSkull";

	public override void SetDefaults()
	{
		Projectile.width = 54;
		Projectile.height = 84;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 10;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void AI()
	{
		ref float ai0 = ref Projectile.ai[0];
		ref float ai1 = ref Projectile.ai[1];

		Player player = Main.player[Projectile.owner];
		Projectile.frame = (int)(Main.timeForVisualEffects % 25 / 5f);
		Projectile.Center = Projectile.Center * 0.7f + player.Center * 0.3f;

		// 面朝方向 X 轴偏移
		Projectile.position.X += player.direction * 32 * 0.3f;
		Projectile.position.Y += -22 * player.gravDir * (float)(1.2 + Math.Sin(Main.time / 18d) / 8d) * 0.3f;

		// 防止玩家移动的时候，弹幕贴脸，XY 轴都会因为移动一点点，但是不会很多
		Projectile.position += player.velocity * 0.7f;
		Projectile.position -= Vector2.Clamp(player.velocity * 0.1f, new Vector2(-10), new Vector2(10));

		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;

		if (!player.controlUseItem)
		{
			if (ai1 > 0)
			{
				ai0 *= 0.9f;
				ai1 -= 1f;
				Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
				Projectile.Center = player.MountedCenter + Vector2.Normalize(toMouse).RotatedBy(ai0 / 4d) * (8f - ai0 * 4);
			}
			else
			{
				if (Projectile.timeLeft > player.itemTime + 1)
				{
					Projectile.timeLeft = player.itemTime;
				}
				for (int j = 0; j < 16; j++)
				{
					Vector2 velocity = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * Projectile.scale;
					Dust dust = Dust.NewDustDirect(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<MothSmog>(), 0, 0, 100, default, Main.rand.NextFloat(3.7f, 5.1f) * 0.13f);
					dust.alpha = (int)(dust.scale * 0.3);
					dust.rotation = Main.rand.NextFloat(0, 6.283f);
					dust.velocity = velocity;
				}
			}
		}
		else
		{
			Projectile.timeLeft = player.itemTime;
		}
		if (Projectile.timeLeft == 2)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Projectile.Center);
		Player player = Main.player[Projectile.owner];
		var shootVelocity = new Vector2(Math.Sign((Main.MouseWorld - Main.player[Projectile.owner].MountedCenter).X), 0.6f * player.gravDir);
		Vector2 shootCenter = Projectile.Center + new Vector2(0, 16f * player.gravDir);
		ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), shootCenter, shootVelocity * 3, ModContent.ProjectileType<NavyThunderBomb>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 0, 0);
		Vector2 newVelocity = shootVelocity;
		newVelocity *= 1f - Main.rand.NextFloat(0.3f);
		newVelocity *= 2f;

		for (int j = 0; j < 30; j++)
		{
			Vector2 baseVelocity = newVelocity / 27f * j;
			Vector2 velocity = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + baseVelocity;
			Dust dust = Dust.NewDustDirect(shootCenter, 0, 0, ModContent.DustType<BlueGlowAppearStoppedByTile>(), velocity.X, velocity.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * 0.4f);
			dust.noGravity = true;
		}
		for (int j = 0; j < 30; j++)
		{
			Vector2 baseVelocity = newVelocity / 54f * j;
			Vector2 velocity = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + baseVelocity;
			float Scale = Main.rand.NextFloat(3.7f, 5.1f);
			Dust dust = Dust.NewDustDirect(shootCenter + new Vector2(4, 4.5f), 0, 0, ModContent.DustType<BlueParticleDark2StoppedByTile>(), velocity.X, velocity.Y, 100, default, Scale);
			dust.alpha = (int)(dust.scale * 50);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		player.heldProj = Projectile.whoAmI;
		Vector2 toProjectile = Projectile.Center - player.MountedCenter;

		if (player.controlUseItem)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(toProjectile.Y, toProjectile.X) - Math.PI / 2d));
		}

		Texture2D texMain = ModAsset.FlameSkull.Value;
		Texture2D texMainG = ModAsset.FlameSkullGlow.Value;

		var drawRect = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);

		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects effect = SpriteEffects.None;
		if (player.direction == 1)
		{
			effect = SpriteEffects.FlipHorizontally;
		}

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, drawRect, drawColor, Projectile.rotation, drawRect.Size() / 2f, 1f, effect, 0);
		Main.spriteBatch.Draw(texMainG, Projectile.Center - Main.screenPosition, drawRect, new Color(255, 255, 255, 0), Projectile.rotation, drawRect.Size() / 2f, 1f, effect, 0);
	}

	public void DrawWarp(VFXBatch sb)
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		Vector2 toProjectile = Projectile.Center - player.MountedCenter;

		if (player.controlUseItem)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(toProjectile.Y, toProjectile.X) - Math.PI / 2d));
		}

		Texture2D texMainG = ModAsset.FlameSkullWarp.Value;
		var drawRect = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);

		SpriteEffects effect = SpriteEffects.None;
		if (player.direction == 1)
		{
			effect = SpriteEffects.FlipHorizontally;
		}

		sb.Draw(texMainG, Projectile.Center - Main.screenPosition, drawRect, new Color(0.3f, 0.03f, 0.2f, 0), Projectile.rotation, drawRect.Size() / 2f, 1f, effect);
	}
}
