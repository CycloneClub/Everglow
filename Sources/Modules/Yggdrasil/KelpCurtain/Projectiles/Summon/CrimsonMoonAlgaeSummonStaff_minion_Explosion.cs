using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class CrimsonMoonAlgaeSummonStaff_minion_Explosion : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 60;
		Projectile.height = 60;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.timeLeft = 60;
		Projectile.aiStyle = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		var gasRing = new RedAlgae_GasRing();
		gasRing.Position = Projectile.Center;
		gasRing.MaxTime = 60;
		gasRing.Scale = 14f;
		gasRing.Visible = true;
		gasRing.Active = true;
		Ins.VFXManager.Add(gasRing);
		float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
		float randomSize = Main.rand.NextFloat(0.85f, 1.15f);
		for (int k = 0; k < 36; k++)
		{
			var redAlgaeDust = new RedAlgae_Spark();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, 12 * randomSize).RotatedBy(MathHelper.TwoPi * k / 18f + randomRot);
			redAlgaeDust.ai = new float[] { 0.9f };
			redAlgaeDust.MaxTime = 50;
			redAlgaeDust.Scale = Main.rand.NextFloat(4.7f, 8f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		for (int k = 0; k < 18; k++)
		{
			var redAlgaeDust = new RedAlgae_Spark();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, 6 * randomSize).RotatedBy(MathHelper.TwoPi * k / 18f + randomRot);
			redAlgaeDust.ai = new float[] { 0.9f };
			redAlgaeDust.MaxTime = 50;
			redAlgaeDust.Scale = Main.rand.NextFloat(4.7f, 8f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		for (int k = 0; k < 20; k++)
		{
			var redAlgaeDust = new RedAlgae_Small_Dust();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, Main.rand.NextFloat(8f, 24f)).RotatedByRandom(MathHelper.TwoPi);
			redAlgaeDust.ai = new float[] { 0.9f };
			redAlgaeDust.Frame = Main.rand.Next(10);
			redAlgaeDust.MaxTime = 60;
			redAlgaeDust.Scale = Main.rand.NextFloat(1.7f, 2f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		var star = new RedAlgaeHitStar();
		star.Position = Projectile.Center;
		star.MaxTime = 30;
		star.Scale = 2.2f;
		star.Visible = true;
		star.Active = true;
		Ins.VFXManager.Add(star);
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int type = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (!target.HasBuff(type))
		{
			target.AddBuff(type, 900);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return MathUtils.IntersectsCircleAABB(Projectile.Center, 120, targetHitbox);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
