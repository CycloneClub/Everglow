namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class AleBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +50% whip range
		player.whipRangeMultiplier += 0.5f;
		// +50% summon attack speed
		player.GetAttackSpeed(DamageClass.Summon) += 0.5f;
	}
}
