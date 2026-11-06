namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class CreamSodaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.aggro -= 2400;
		player.GetCritChance(DamageClass.Melee) += 40; // 加40%暴击
		// +40% melee damage
		player.GetDamage(DamageClass.Melee) += 0.4f;
		player.GetAttackSpeed(DamageClass.Generic) += 0.4f; // 加40%攻速
	}
}
