using Everglow.Yggdrasil.KelpCurtain.CustomTiles;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs;

public class BlackAwningBoat_WaterDistort : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public BlackAwningBoat ParentBoat;

	public override void SetDefaults()
	{
		Projectile.width = 90;
		Projectile.height = 64;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.timeLeft = 600000;
		base.SetDefaults();
	}

	public override void AI()
	{
		if (ParentBoat is null || !ParentBoat.Active || ParentBoat.Velocity.Length() <= 0.001f)
		{
			Projectile.active = false;
			return;
		}
		else
		{
			Projectile.Center = new Vector2(ParentBoat.Box.Center.X + MathF.Sign(ParentBoat.Velocity.X) * 64, ParentBoat.Box.Bottom);
			Projectile.velocity = ParentBoat.Velocity * 2;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
