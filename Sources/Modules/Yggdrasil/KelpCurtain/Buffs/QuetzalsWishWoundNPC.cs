using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class QuetzalsWishWoundNPC : GlobalNPC
{
	public override void UpdateLifeRegen(NPC npc, ref int damage)
	{
		if (!npc.HasBuff(ModContent.BuffType<QuetzalsWishWound>()) || npc.velocity == Vector2.Zero)
		{
			return;
		}
		npc.lifeRegen = QuetzalsWishCombatRules.WoundedLifeRegen(npc.lifeRegen, npc.velocity.X, npc.velocity.Y);
		damage = Math.Max(damage, 3);
	}
}
