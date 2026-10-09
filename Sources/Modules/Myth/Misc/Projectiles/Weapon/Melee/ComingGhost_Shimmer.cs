using Everglow.Myth.Misc.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class ComingGhost_Shimmer : ModProjectile
{
	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;
	public override void OnSpawn(IEntitySource source)
	{
		// Projectile.ai[1] = Main.player[Projectile.owner].direction;
	}
	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 2;
	}
	public override void AI()
	{
		Projectile.velocity *= 0f;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Color c0 = new Color(1f, 0, 0, 0);
		float energyValue = Projectile.timeLeft / 300f;
		energyValue *= energyValue;
		Texture2D dark = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, 0 + Projectile.ai[0], dark.Size() / 2f, new Vector2(6f * energyValue, 2), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, MathHelper.PiOver2 + Projectile.ai[0], dark.Size() / 2f, new Vector2(5f * energyValue, 3), SpriteEffects.None, 0);

		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, 0 + Projectile.ai[0], light.Size() / 2f, new Vector2(6f * energyValue, 2), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, MathHelper.PiOver2 + Projectile.ai[0], light.Size() / 2f, new Vector2(5f * energyValue, 3), SpriteEffects.None, 0);
		return false;
	}
}
