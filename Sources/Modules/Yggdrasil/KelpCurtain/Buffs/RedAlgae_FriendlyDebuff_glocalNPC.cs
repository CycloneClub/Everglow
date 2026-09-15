
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class RedAlgae_FriendlyDebuff_glocalNPC : GlobalNPC
{
	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		DoDamageRedAlgaeBuff(npc);
		base.OnHitByProjectile(npc, projectile, hit, damageDone);
	}

	public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		DoDamageRedAlgaeBuff(npc);
		base.OnHitByItem(npc, player, item, hit, damageDone);
	}

	private void DoDamageRedAlgaeBuff(NPC npc)
	{
		int buffType = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (npc.HasBuff(buffType))
		{
			int index = npc.FindBuffIndex(buffType);
			int buffTime = npc.buffTime[index];
			int damage = 900 - buffTime;
			if (damage > 10)
			{
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
