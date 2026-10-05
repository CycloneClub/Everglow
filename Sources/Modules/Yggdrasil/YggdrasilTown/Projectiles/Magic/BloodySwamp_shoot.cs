using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BloodySwamp_shoot : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 12000;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		var colorLight = new Vector3(0.9f, 0.0f, 0.12f);
		Vector2 velocity = new Vector2(0, 2).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) + Projectile.velocity * 0.25f;
		var somg = new BloodSwampDust
		{
			Velocity = velocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = 90,
			Scale = 25,
			Rotation = Main.rand.NextFloat(6.283f),
			MaxScale = Main.rand.NextFloat(12.0f, 28.0f),
			ChasedProjectile = Projectile,
			ai = new float[] { Main.rand.NextFloat(-0.12f, 0.12f), Main.rand.NextFloat(MathHelper.TwoPi), 0 },
		};
		Ins.VFXManager.Add(somg);
		Lighting.AddLight(Projectile.Center, colorLight);
		base.AI();
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
