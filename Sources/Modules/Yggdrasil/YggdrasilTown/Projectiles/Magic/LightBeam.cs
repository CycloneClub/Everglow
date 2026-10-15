using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

internal class LightBeam : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	private const int InitialDuration = 150;
	private const int MaxDuration = 360;
	private const int DurationAddedPerHit = 90;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.magic = true;
		Projectile.extraUpdates = 100;
		Projectile.timeLeft = 200;
		Projectile.penetrate = 1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < 60; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Vector2 newVelocity = new Vector2(0, 0.7f).RotatedBy(i / 60f * MathHelper.TwoPi);
				newVelocity.X *= 0.6f * MathF.Sin(i / 45f * MathHelper.TwoPi) + 0.2f;
				newVelocity = newVelocity.RotatedBy(-Main.time * 0.05f + Projectile.whoAmI + j * MathHelper.TwoPi / 3f);
				var somg = new LightFruitParticleDust
				{
					Velocity = newVelocity * 0.4f,
					Active = true,
					Visible = true,
					Position = Projectile.Center + newVelocity * 20,
					MaxTime = Main.rand.Next(37, 145),
					Scale = Main.rand.NextFloat(12.20f, 32.35f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(1, 4f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
		}
	}

	public override void AI()
	{
		GenerateParticles(12);
	}

	public void GenerateParticles(int duplicateTimes = 1)
	{
		float mulMaxTime = 1f;
		if (Projectile.timeLeft > 1100)
		{
			mulMaxTime = 1f - (Projectile.timeLeft - 1100) / 100f;
		}
		if (Projectile.timeLeft < 150)
		{
			mulMaxTime = (Projectile.timeLeft - 90f) / 60f;
		}
		if (mulMaxTime < 0)
		{
			return;
		}
		for (int i = 0; i < duplicateTimes; i++)
		{
			Vector2 newVelocity = Vector2.zeroVector;
			var somg = new LightFruitParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity * Main.rand.NextFloat(1f),
				MaxTime = Main.rand.Next(37, 145) * mulMaxTime,
				Scale = Main.rand.NextFloat(12.20f, 32.35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(1f, 10f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int buffType = ModContent.BuffType<Photolysis>();

		if (!target.HasBuff(buffType))
		{
			target.AddBuff(buffType, InitialDuration);
		}
		else
		{
			int buffIndex;
			for (buffIndex = 0; buffIndex < target.buffType.Length; buffIndex++)
			{
				if (buffType == target.buffType[buffIndex])
				{
					break;
				}
			}

			if (buffIndex >= target.buffType.Length)
			{
				return;
			}

			if (target.buffTime[buffIndex] + DurationAddedPerHit <= MaxDuration)
			{
				target.buffTime[buffIndex] += DurationAddedPerHit;
			}
			else
			{
				target.buffTime[buffIndex] = MaxDuration;
			}
		}

		for (int i = 0; i < target.buffType.Length; i++)
		{
			if (buffType == target.buffType[i])
			{
				return;
			}
		}
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 20; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LampWood_Dust_fluorescent_appear>());
			dust.alpha = 0;
			dust.rotation = Main.rand.NextFloat(0.3f, 0.7f);
			dust.scale *= 2f;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.oldPosition + new Vector2(4), Vector2.zeroVector, ModContent.ProjectileType<LightStartEffect_beam>(), 0, 0, Projectile.owner);

		var lightStartEffectProjectiles = new List<Projectile>();

		// Collect all active LightStartEffect_beam projectiles
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active && proj.type == ModContent.ProjectileType<LightStartEffect_beam>())
			{
				lightStartEffectProjectiles.Add(proj);
			}
		}

		// Sort the collected projectiles by timeLeft in descending order
		lightStartEffectProjectiles.Sort((a, b) => b.timeLeft.CompareTo(a.timeLeft));

		// Get the top 5 projectiles (if they exist)
		var topFiveProjectiles = new HashSet<Projectile>();
		for (int i = 0; i < Math.Min(5, lightStartEffectProjectiles.Count); i++)
		{
			topFiveProjectiles.Add(lightStartEffectProjectiles[i]);
		}

		// Set timeLeft to 150 for other projectiles with timeLeft > 150
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active && proj.type == ModContent.ProjectileType<LightStartEffect_beam>() && proj.timeLeft > 150 && !topFiveProjectiles.Contains(proj))
			{
				proj.timeLeft = 150;
			}
		}
	}
}
