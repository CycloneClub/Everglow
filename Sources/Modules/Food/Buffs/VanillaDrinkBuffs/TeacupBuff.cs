namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class TeacupBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +60% magic damage
		player.GetDamage(DamageClass.Magic) += 0.6f;
		player.GetCritChance(DamageClass.Magic) += 60; // 加60%暴击
		// +100 mana regen
		player.manaRegenBonus += 100;
	}
}
