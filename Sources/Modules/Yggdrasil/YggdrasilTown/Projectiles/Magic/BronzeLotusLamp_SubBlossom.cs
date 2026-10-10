using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BronzeLotusLamp_SubBlossom : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 8;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
		Projectile.tileCollide = false;
	}

	public override void AI()
	{
		float value = 1 - Projectile.timeLeft / 60f;
		float speed = MathF.Sin(value * MathF.PI * 1.5f) + 1;
		speed *= 0.6f;
		float size = Main.rand.NextFloat(0.1f, 0.96f);
		float maxTime = Main.rand.Next(24, 36);
		if (Projectile.ai[0] == 1)
		{
			maxTime = Main.rand.Next(16, 20);
		}
		var lotusFlame = new CyanLotusFlameDust
		{
			Velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * speed,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = maxTime,
			Scale = 8f * size,
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			Frame = Main.rand.Next(3),
			ai = new float[] { Main.rand.NextFloat(-0.8f, 0.8f) },
		};
		Ins.VFXManager.Add(lotusFlame);

		var lotusFlame2 = new CyanLotusFlameDust
		{
			Velocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * speed,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = maxTime,
			Scale = 8f * size,
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			Frame = Main.rand.Next(3),
			ai = new float[] { Main.rand.NextFloat(-0.8f, 0.8f) },
		};
		Ins.VFXManager.Add(lotusFlame2);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}
