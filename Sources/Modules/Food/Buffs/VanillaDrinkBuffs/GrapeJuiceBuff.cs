namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class GrapeJuiceBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +10000 luck
		player.equipmentBasedLuckBonus += 10000f;
	}
}
