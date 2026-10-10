using Everglow.Commons.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_Slash_Shine : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 6;

		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
	}

	public override void AI()
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeLeftValue = 1 - Projectile.timeLeft / 120f;
		float width = 1 - MathF.Pow(timeLeftValue, 2);
		Texture2D starBlack = Commons.ModAsset.Star2_black.Value;
		Texture2D star = Commons.ModAsset.StabbingProjectile.Value;
		Main.EntitySpriteDraw(starBlack, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, starBlack.Size() * 0.5f, new Vector2(1.6f + width, 0.8f * width), SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, new Color(0.2f, 0f, 0.9f, 0), Projectile.rotation, star.Size() * 0.5f, new Vector2(1.6f + width, 0.8f * width), SpriteEffects.None, 0);
		return false;
	}
}
