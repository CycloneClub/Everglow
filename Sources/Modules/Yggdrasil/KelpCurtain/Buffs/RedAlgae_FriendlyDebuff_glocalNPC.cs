
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class RedAlgae_FriendlyDebuff_glocalNPC : GlobalNPC
{
	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		// Resolve the attacking player from the projectile owner, guarding the index so an
		// unowned or out-of-range owner cannot throw.
		Player attacker = null;
		if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
		{
			attacker = Main.player[projectile.owner];
		}
		ApplyRedAlgaeDetonation(npc, attacker);
		base.OnHitByProjectile(npc, projectile, hit, damageDone);
	}

	public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		ApplyRedAlgaeDetonation(npc, player);
		base.OnHitByItem(npc, player, item, hit, damageDone);
	}

	/// <summary>
	/// Detonates the accumulated <see cref="RedAlgae_FriendlyDebuff"/> on <paramref name="npc"/>.
	/// The 红月水藻 set bonus multiplies the detonation damage by 2.5 (design:
	/// 引爆红藻毒素的伤害提高150%) when the resolved <paramref name="attacker"/> wears the set.
	/// </summary>
	private void ApplyRedAlgaeDetonation(NPC npc, Player attacker)
	{
		int buffType = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (npc.HasBuff(buffType))
		{
			int index = npc.FindBuffIndex(buffType);
			int buffTime = npc.buffTime[index];
			int damage = 900 - buffTime;
			if (damage > 10)
			{
				bool setBonus = attacker != null
					&& attacker.active
					&& attacker.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeSetBuff;
				if (setBonus)
				{
					damage = (int)(damage * 2.5f);
				}

				NPC.HitInfo hit2 = new NPC.HitInfo()
				{
					Damage = damage,
					Knockback = 0,
					HitDirection = 1,
					Crit = false,
				};
				npc.StrikeNPCWithCustomCombatText(hit2, new Color(0.7f, 0.1f, 0.4f), true);
				npc.DelBuff(index);
			}
		}
	}
}
