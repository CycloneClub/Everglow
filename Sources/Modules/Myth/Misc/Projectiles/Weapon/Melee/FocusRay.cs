using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class FocusRay : ModProjectile
{
	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 2;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
	}

	public NPC StickTarget;
	public Projectile Yoyo;

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		if (StickTarget == null)
		{
			Projectile.active = false;
			return;
		}
		Projectile.Center = StickTarget.Center;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float energyValue = Projectile.timeLeft / 60f;

		Color c0 = new Color(energyValue, energyValue * energyValue, energyValue * energyValue * 0.5f, 0);
		energyValue *= energyValue;
		Vector2 size = new Vector2(2f, 2 * energyValue) * 0.7f * Projectile.scale;
		Texture2D dark = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, Projectile.rotation, dark.Size() / 2f, size, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, Projectile.rotation + MathHelper.Pi / 3f, dark.Size() / 2f, size, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, Projectile.rotation + MathHelper.Pi / 3f * 2f, dark.Size() / 2f, size, SpriteEffects.None, 0);

		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, light.Size() / 2f, size, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation + MathHelper.Pi / 3f, light.Size() / 2f, size, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation + MathHelper.Pi / 3f * 2f, light.Size() / 2f, size, SpriteEffects.None, 0);
		return false;
	}
}
