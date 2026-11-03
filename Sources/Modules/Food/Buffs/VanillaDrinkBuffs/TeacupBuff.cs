namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class TeacupBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +60% magic damage
		player.GetDamage(DamageClass.Magic) += 0.6f;
		player.GetCritChance(DamageClass.Magic) += 60; // 加60%暴击
		// +100 mana regen
		player.manaRegenBonus += 100;
	}
}
