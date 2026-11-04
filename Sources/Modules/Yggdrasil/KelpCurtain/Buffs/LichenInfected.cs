using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;
using Terraria.DataStructures;

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
			AddProjectile(npc, player.GetSource_OnHit(npc), player.whoAmI);
		}
		base.OnHitByItem(npc, player, item, hit, damageDone);
	}

	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		if (npc.HasBuff(ModContent.BuffType<LichenInfected>()))
		{
			AddProjectile(npc, projectile.GetSource_OnHit(npc), projectile.owner);
		}
		base.OnHitByProjectile(npc, projectile, hit, damageDone);
	}

	public void AddProjectile(NPC npc, IEntitySource source, int owner)
	{
		bool hasProj = false;
		if (npc.HasBuff(ModContent.BuffType<LichenInfected>()))
		{
			foreach (Projectile p in Main.projectile)
			{
				if (p.active && p.type == ModContent.ProjectileType<LichensPycnidium>())
				{
					if (p.owner == owner)
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
				Projectile p0 = Projectile.NewProjectileDirect(source, npc.Center, Vector2.zeroVector, ModContent.ProjectileType<LichensPycnidium>(), 150, 0, owner, Main.rand.NextFloat(MathHelper.TwoPi));
				LichensPycnidium lp = p0.ModProjectile as LichensPycnidium;
				if (lp != null)
				{
					lp.AddLichens(Main.rand.NextFloat(MathHelper.TwoPi), npc, Main.rand.NextFloat(-20f, 20f));
				}
			}
		}
	}
}
