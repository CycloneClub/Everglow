using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Audio;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class GlowBeadGun : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.GlowBeadGun_Mod;

	public Player Owner => Main.player[Projectile.owner];

	public Vector2 OwnerMouseWorld => Owner.MouseWorld();

	public ref float AI0 => ref Projectile.ai[0];

	public ref float AI1 => ref Projectile.ai[1];

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 100000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
	}

	private Vector2 oldPo = Vector2.Zero;
	private int energy = 0;

	public override bool PreAI()
	{
		if (Projectile.timeLeft <= 10)
		{
			if (Projectile.timeLeft > 3)
			{
				energy = Math.Min(energy += 20, 180);
			}
			else
			{
				energy = 0;
			}
			Projectile.rotation -= Owner.direction * Projectile.timeLeft * AI1 / 72000f;
			Vector2 v0 = OwnerMouseWorld - Owner.MountedCenter;
			v0 = Vector2.Normalize(v0);
			Projectile.Center = Owner.MountedCenter + Vector2.Normalize(v0).RotatedBy(AI0 / 0.8d) * (8f - AI0 * 8);
			return false;
		}
		return true;
	}

	public override void AI()
	{
		if (Projectile.timeLeft % 5 == 0 && energy < 180)
		{
			Owner.statMana--;
		}

		energy = Math.Min(++energy, 180);
		Vector2 v0 = OwnerMouseWorld - Owner.MountedCenter;
		v0 = Vector2.Normalize(v0);
		if (Owner.controlUseItem)
		{
			AI0 *= 0.9f;
			AI1 -= 1f;
			Projectile.rotation = (float)(Math.Atan2(v0.Y, v0.X) + Math.PI * 0.25);
			Projectile.Center = Owner.MountedCenter + Vector2.Normalize(v0).RotatedBy(AI0 / 0.8d) * (8f - AI0 * 8);
			oldPo = Projectile.Center;
			Projectile.Center = oldPo;
			Projectile.velocity *= 0;
		}
		if (!Owner.controlUseItem)
		{
			if (AI1 > 0)
			{
				AI0 *= 0.9f;
				AI1 -= 1f;
				Projectile.Center = Owner.MountedCenter + Vector2.Normalize(v0).RotatedBy(AI0 / 4d) * (8f - AI0 * 4);
			}
			else
			{
				Shoot();
				return;
			}
		}
		if (Owner.statMana <= 0 || Projectile.timeLeft < 2)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		Vector2 offset = new Vector2(0, -15f);
		SoundEngine.PlaySound(SoundID.Item72, Projectile.Center);
		Vector2 v0 = OwnerMouseWorld - Owner.MountedCenter;
		v0 = Vector2.Normalize(v0);
		ScreenShaker Gsplayer = Owner.GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, 2 * energy).RotatedByRandom(6.283);
		int NumProjectiles = (int)(energy / 20f) + 1;
		for (int i = 0; i < NumProjectiles; i++)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v0 * 62 + offset, v0 * (12 - i) * (energy + 20) / 240f, ModContent.ProjectileType<GlowStar>(), (int)(Projectile.damage / (i + 19f) * 19f * (energy + 120) / 180f), Projectile.knockBack, Owner.whoAmI, i * (energy + 20) / 180f, 0);
		}

		Vector2 newVelocity = v0;
		newVelocity *= 1f - Main.rand.NextFloat(0.3f);
		newVelocity *= Math.Clamp(energy / 18f, 0.2f, 10f);
		Vector2 basePos = Projectile.Center + newVelocity * 3.7f + v0 * 62;

		for (int j = 0; j < energy; j++)
		{
			Vector2 v = newVelocity / 27f * j;
			Vector2 v1 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
			int num20 = Dust.NewDust(basePos + offset, 0, 0, ModContent.DustType<BlueGlowAppearStoppedByTile>(), v1.X, v1.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * 0.4f);
			Main.dust[num20].noGravity = true;
		}
		for (int j = 0; j < energy; j++)
		{
			Vector2 v = newVelocity / 54f * j;
			Vector2 v1 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
			float Scale = Main.rand.NextFloat(3.7f, 5.1f);
			int num21 = Dust.NewDust(basePos + offset + new Vector2(4, 4.5f), 0, 0, ModContent.DustType<BlueParticleDark2StoppedByTile>(), v1.X, v1.Y, 100, default, Scale);
			Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
		}
		for (int j = 0; j < 16; j++)
		{
			Vector2 v = newVelocity / 54f * j;
			Vector2 v1 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
			v1 *= 0.2f;
			float Scale = Main.rand.NextFloat(3.7f, 5.1f);
			int num21 = Dust.NewDust(basePos + offset + new Vector2(4, 4.5f), 0, 0, ModContent.DustType<MothSmog>(), v1.X, v1.Y, 100, default, Scale);
			Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
		}
		Owner.velocity -= newVelocity * 0.1f;
		AI1 = energy;
		Projectile.timeLeft = 10;
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + v0 * 52f + offset, Vector2.Zero, ModContent.ProjectileType<GlowBeadGunShootFlame>(), 0, 0, Owner.whoAmI, energy / 450f + 0.1f, Projectile.rotation - MathF.PI / 4);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Owner.heldProj = Projectile.whoAmI;
		Vector2 v0 = Projectile.Center - Owner.MountedCenter;
		if (Main.mouseLeft)
		{
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(v0.Y, v0.X) - Math.PI / 2d));
		}

		Texture2D TexMain = ModAsset.GlowBeadGunOff.Value;
		Texture2D TexMainG = ModAsset.GlowBeadGunGlow.Value;
		Projectile.frame = (int)(energy % 45 / 5f);

		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (Projectile.Center.X < Owner.Center.X)
		{
			se = SpriteEffects.FlipVertically;
			Owner.direction = -1;
		}
		else
		{
			Owner.direction = 1;
		}
		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition - new Vector2(0, 6), null, drawColor, Projectile.rotation - (float)(Math.PI * 0.25) + (float)(AI0 / -3d) * Owner.direction, new Vector2(35, 22), 1f, se, 0);
		Main.spriteBatch.Draw(TexMainG, Projectile.Center - Main.screenPosition - new Vector2(0, 6), new Rectangle(0, 0, (int)(energy / 180f * 74f) + 30, TexMainG.Height), new Color(255, 255, 255, 0), Projectile.rotation - (float)(Math.PI * 0.25) + (float)(AI0 / -3d) * Owner.direction, new Vector2(35, 22), 1f, se, 0);
	}
}
