using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class LichenInfected : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		int buffDamage = (int)(5 + npc.velocity.Length() * 8);

		npc.lifeRegen = -buffDamage;
		npc.SetLifeRegenExpectedLossPerSecond((int)(1 + npc.velocity.Length() * 2.4f));

		base.Update(npc, ref buffIndex);
	}
}

public class LichenInfectedNPC : GlobalNPC
{
	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
	{
		if (npc.HasBuff(ModContent.BuffType<LichenInfected>()))
		{
		}
		base.OnHitPlayer(npc, target, hurtInfo);
	}

	public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		if (npc.HasBuff(ModContent.BuffType<LichenInfected>()))
		{
			AddProjectile(npc, player);
		}
		base.OnHitByItem(npc, player, item, hit, damageDone);
	}

	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		if (npc.HasBuff(ModContent.BuffType<LichenInfected>()))
		{
			AddProjectile(npc, Main.player[projectile.owner]);
		}
		base.OnHitByProjectile(npc, projectile, hit, damageDone);
	}

	public void AddProjectile(NPC npc, Player player)
	{
		bool hasProj = false;
		if (npc.HasBuff(ModContent.BuffType<LichenInfected>()))
		{
			foreach (Projectile p in Main.projectile)
			{
				if (p.active && p.type == ModContent.ProjectileType<LichensPycnidium>())
				{
					if (p.owner == player.whoAmI)
					{
						LichensPycnidium lP = p.ModProjectile as LichensPycnidium;
						if (lP != null)
						{
							lP.AddLichens(Main.rand.NextFloat(MathHelper.TwoPi), npc, Main.rand.NextFloat(-200f, 200f));
							hasProj = true;
							break;
						}
					}
				}
			}
			if (!hasProj)
			{
				Projectile p0 = Projectile.NewProjectileDirect(player.GetSource_FromAI(), npc.Center, Vector2.zeroVector, ModContent.ProjectileType<LichensPycnidium>(), 150, 0, player.whoAmI, Main.rand.NextFloat(MathHelper.TwoPi));
				LichensPycnidium lp = p0.ModProjectile as LichensPycnidium;
				if (lp != null)
				{
					lp.AddLichens(Main.rand.NextFloat(MathHelper.TwoPi), npc, Main.rand.NextFloat(-20f, 20f));
				}
			}
		}
	}
}
