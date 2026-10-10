using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;

public class JellyBallCrystal : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 1200;
	}

	public void GenerateParticles(int duplicateTimes = 1)
	{
	}

	public override void AI()
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathF.PI, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnKill(int timeLeft)
	{
	}
}
