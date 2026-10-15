using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;

public class SpongeOxygenBubble : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120000;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = false;
	}

	public override void AI()
	{
		Projectile.velocity.Y -= 0.05f;
		Projectile.velocity *= 0.99f;
		Projectile.rotation = 0;
		Projectile.frameCounter++;
		if (Projectile.frameCounter >= 7)
		{
			Projectile.frameCounter = 0;
			++Projectile.frame;
			if (Projectile.frame >= 4)
			{
				Projectile.frame = 0;
			}
		}
		if (!Projectile.wet)
		{
			Projectile.active = false;
		}
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		modifiers.Cancel();
		target.breath += Projectile.damage;
		Projectile.Kill();
		base.ModifyHitPlayer(target, ref modifiers);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Rectangle frame = new Rectangle(0, Projectile.frame * 22, 22, 22);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
	}
}
