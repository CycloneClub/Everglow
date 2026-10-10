using Everglow.Myth.Common;
using Everglow.Myth.TheTusk;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

internal class XiaoHit : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 68;
		Projectile.height = 68;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int x = 0; x < 5; x++)
		{
			GenerateVFX();
		}
	}

	private void GenerateVFX()
	{
		Player player = Main.player[Projectile.owner];
		var velocity = new Vector2(0, Main.rand.NextFloat(12f, 20f) * Projectile.ai[0]).RotatedByRandom(6.283);
		var positionVFX = Projectile.Center - velocity;

		var filthy = new FilthyLucreFlame_darkDust
		{
			velocity = velocity,
			Active = true,
			Visible = true,
			position = positionVFX,
			maxTime = Main.rand.Next(17, 56),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.03f, 0.03f), Main.rand.NextFloat(18f, 30f) * Projectile.ai[0] },
		};
		Ins.VFXManager.Add(filthy);
		var filthy2 = new FilthyLucreFlameDust
		{
			velocity = velocity,
			Active = true,
			Visible = true,
			position = positionVFX,
			maxTime = Main.rand.Next(17, 56),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(18f, 30f) * Projectile.ai[0] },
		};
		Ins.VFXManager.Add(filthy2);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}
