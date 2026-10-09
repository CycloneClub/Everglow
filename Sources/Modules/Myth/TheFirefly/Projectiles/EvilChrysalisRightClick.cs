using Everglow.Myth.Common;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class EvilChrysalisRightClick : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.friendly = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.itemTime = 5;
		player.itemAnimation = 5;
		Projectile.position = player.MountedCenter - new Vector2(25, 25);
		player.heldProj = Projectile.whoAmI;
		if (Main.mouseRight && player.statMana >= player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()])
		{
			Projectile.timeLeft = 5;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];

		Vector2 Vdr = Main.MouseWorld - Projectile.Center;

		Vdr = Vdr / Vdr.Length() * 7;

		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(Vdr.Y, Vdr.X) - Math.PI / 2d));
		Texture2D t = ModAsset.EvilChrysalisRightClick.Value;
		Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects S = SpriteEffects.None;
		if (Math.Sign(Vdr.X) == -1)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}

		Main.spriteBatch.Draw(t, player.MountedCenter - Main.screenPosition + Vdr * 5f, null, color, (float)(Math.Atan2(Vdr.Y, Vdr.X) + Math.PI / 4d), new Vector2(25f, 25f), Projectile.scale, S, 0f);

		Texture2D tg = ModAsset.EvilChrysalisG.Value;
		Main.spriteBatch.Draw(tg, player.MountedCenter - Main.screenPosition + Vdr * 5f, null, new Color(255, 255, 255, 0), (float)(Math.Atan2(Vdr.Y, Vdr.X) + Math.PI / 4d), new Vector2(25f, 25f), Projectile.scale, S, 0f);
		return false;
	}
}
