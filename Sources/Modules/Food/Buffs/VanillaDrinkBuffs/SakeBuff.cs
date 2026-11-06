namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class SakeBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense -= 18; // 减18防御
		player.GetCritChance(DamageClass.Melee) += 40; // 加40%暴击
		// +40% melee damage
		player.GetDamage(DamageClass.Melee) += 0.4f;
		player.GetAttackSpeed(DamageClass.Generic) += 0.4f; // 加40%攻速
	}
}
