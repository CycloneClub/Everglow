using Terraria.DataStructures;

namespace Everglow.MEAC.Projectiles;

public class VortexVanquisherThump : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => "Everglow/MEAC/Projectiles/VortexVanquisher";

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 3;
	}

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.netImportant = true;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.extraUpdates = 10;
	}

	public Vector2 StartVelocity;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.velocity = Vector2.Normalize(Projectile.velocity);
		StartVelocity = Projectile.velocity;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.extraUpdates = (int)(10 * player.meleeSpeed);
		if (Projectile.timeLeft % 40 == 0)
		{
			StrikeDown();
		}

		player.immune = true;
		player.immuneTime = 8;
		Projectile.velocity = StartVelocity;
		Projectile.position += new Vector2(18f / Projectile.extraUpdates * Math.Sign(StartVelocity.X), 0);
		if (Projectile.timeLeft > 20)
		{
			player.velocity = StartVelocity * 24f;
		}
		else
		{
			player.velocity *= 0.6f;
			if (Projectile.timeLeft == 1)
			{
				Projectile.Kill();
			}
		}
	}

	public void StrikeDown()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 CheckPoint = Projectile.Center + new Vector2(0, -100) * player.gravDir;

		for (int y = 0; y < 60; y++)
		{
			if (Collision.SolidCollision(CheckPoint, 1, 1))
			{
				break;
			}
			else
			{
				CheckPoint += new Vector2(0, 5) * player.gravDir;
			}
			if (y == 59)
			{
				CheckPoint = Projectile.Center + new Vector2(0, -100);
			}
		}

		Vector2 TotalVector = Vector2.Zero; // 合向量
		if ((Projectile.Center + new Vector2(0, -100)).Y < CheckPoint.Y)
		{
			int TCount = 0;
			for (int a = 0; a < 12; a++)
			{
				Vector2 v0 = new Vector2(10, 0).RotatedBy(a / 6d * Math.PI);
				if (Collision.SolidCollision(CheckPoint + v0, 1, 1))
				{
					TotalVector -= v0;
					TCount++;
				}
				else
				{
					TotalVector += v0;
				}
			}
			for (int a = 0; a < 24; a++)
			{
				Vector2 v0 = new Vector2(20, 0).RotatedBy(a / 12d * Math.PI);
				if (Collision.SolidCollision(CheckPoint + v0, 1, 1))
				{
					TotalVector -= v0 * 0.5f;
					TCount++;
				}
				else
				{
					TotalVector += v0 * 0.5f;
				}
			}
		}

		if (TotalVector == Vector2.Zero)
		{
			TotalVector = new Vector2(0, -player.gravDir);
		}
		else
		{
			TotalVector = Utils.SafeNormalize(TotalVector, new Vector2(0, -player.gravDir));
		}
		float FallVelocity = 0;
		if ((Projectile.Center + new Vector2(0, -100)).Y < CheckPoint.Y)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), CheckPoint + TotalVector * 480, -TotalVector * 15, ModContent.ProjectileType<DashingLightEff>(), 0, 0, Projectile.owner, 1).CritChance = Projectile.CritChance;
		}
		else
		{
			Vector2 CheckPointII = Projectile.Center + new Vector2(0, 200) * player.gravDir;
			for (int y = 0; y < 600; y++)
			{
				if (Collision.SolidCollision(CheckPointII, 1, 1))
				{
					break;
				}
				else
				{
					CheckPointII += new Vector2(0, 5) * player.gravDir;
					FallVelocity += 0.2f;
				}
			}
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), CheckPoint + TotalVector * 480, -TotalVector * (15 + FallVelocity), ModContent.ProjectileType<DashingLightEff>(), 0, 0, Projectile.owner, 1).CritChance = Projectile.CritChance;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), CheckPoint + TotalVector * 180, -TotalVector * (0.9f + FallVelocity * 0.06f), ModContent.ProjectileType<VortexVanquisher3>(), (int)(Projectile.damage * (1 + FallVelocity * 0.02f)), 0, player.whoAmI, 1).CritChance = Projectile.CritChance;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
