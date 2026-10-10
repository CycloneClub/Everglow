
namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlueMissilFriendly : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/BlueMissil";

	public override void SetDefaults()
	{
		Projectile.width = 34;
		Projectile.height = 34;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 120;
		Projectile.tileCollide = true;
		Projectile.usesLocalNPCImmunity = false;
	}

	public override void AI()
	{
		Projectile.velocity *= 0.98f;
		float lightValue = Projectile.timeLeft / 120f;
		Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<Dusts.BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.7f, 1.9f) * lightValue);
		dust.velocity = Projectile.velocity * 0.8f;
		Projectile.scale *= 0.98f;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 18; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.6f * Projectile.scale);
		}
		for (int i = 0; i < 6; i++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f) * Projectile.scale);
			dust.velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283);
			dust.noGravity = true;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float lightValue = Projectile.timeLeft / 120f;
		Texture2D Light = ModAsset.FixCoinLight3.Value;
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(lightValue, lightValue, lightValue, 0), Projectile.rotation, Light.Size() / 2, Projectile.scale * 2, SpriteEffects.None, 0);
		Texture2D Star = ModAsset.BlueMissil.Value;
		Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(255, 255, 255, 0), 0, Star.Size() * 0.5f, Projectile.scale * 2, SpriteEffects.None, 0);
		return false;
	}
}
