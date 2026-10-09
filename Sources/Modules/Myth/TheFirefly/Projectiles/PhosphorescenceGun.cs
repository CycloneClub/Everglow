using Everglow.Myth.Common;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class PhosphorescenceGun : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/PhosphorescenceGunTex/PhosphorescenceGun";

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 90000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
	}

	private bool release = true;
	private Vector2 oldPo = Vector2.Zero;
	private int addi = 0;

	public override void AI()
	{
		addi++;
		Vector2 v0 = Main.MouseWorld - Main.player[Projectile.owner].MountedCenter;
		if (Main.mouseLeft && release)
		{
			Projectile.ai[0] *= 0.9f;
			Projectile.ai[1] -= 1f;
			Projectile.rotation = (float)(Math.Atan2(v0.Y, v0.X) + Math.PI * 0.25);
			Projectile.Center = Main.player[Projectile.owner].MountedCenter + Vector2.Normalize(v0).RotatedBy(Projectile.ai[0] / 0.8d) * (8f - Projectile.ai[0] * 8);
			oldPo = Projectile.Center;
			Projectile.Center = oldPo;
			Projectile.velocity *= 0;
		}
		if (!Main.mouseLeft && release)
		{
			if (Projectile.ai[1] > 0)
			{
				Projectile.ai[0] *= 0.9f;
				Projectile.ai[1] -= 1f;
				Projectile.Center = Main.player[Projectile.owner].MountedCenter + Vector2.Normalize(v0).RotatedBy(Projectile.ai[0] / 4d) * (8f - Projectile.ai[0] * 4);
			}
			else
			{
				Projectile.Kill();
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		if (!release)
		{
			return;
		}

		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		Vector2 v0 = Projectile.Center - player.MountedCenter;
		if (Main.mouseLeft)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(v0.Y, v0.X) - Math.PI / 2d));
		}

		Texture2D TexMain = ModAsset.PhosphorescenceGunTex_PhosphorescenceGun.Value;
		Texture2D TexMainG = ModAsset.PhosphorescenceGunGlow.Value;

		Projectile.frame = (int)(addi % 45 / 5f);
		var DrawRect = new Rectangle(0, Projectile.frame * 44, 70, 40);

		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (Projectile.Center.X < player.Center.X)
		{
			se = SpriteEffects.FlipVertically;
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition - new Vector2(0, 6), DrawRect, drawColor, Projectile.rotation - (float)(Math.PI * 0.25) + (float)(Projectile.ai[0] / -3d) * player.direction, new Vector2(35, 22), 1f, se, 0);
		Main.spriteBatch.Draw(TexMainG, Projectile.Center - Main.screenPosition - new Vector2(0, 6), DrawRect, new Color(255, 255, 255, 0), Projectile.rotation - (float)(Math.PI * 0.25) + (float)(Projectile.ai[0] / -3d) * player.direction, new Vector2(35, 22), 1f, se, 0);
	}
}
