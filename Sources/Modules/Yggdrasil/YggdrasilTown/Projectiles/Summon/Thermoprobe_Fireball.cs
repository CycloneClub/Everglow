namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class Thermoprobe_Fireball : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override string Texture => "Terraria/Images/Projectile_0";

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.timeLeft = 200;
		Projectile.extraUpdates = 0;
		Projectile.tileCollide = false;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
		var d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		lightColor = Color.White;
		Texture2D tex = ModContent.Request<Texture2D>("Terraria/Images/Projectile_666").Value;
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation() + 1.57f, new Vector2(tex.Width, tex.Height) / 2, Projectile.scale, 0, 0);

		return true;
	}
}
