namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SalmonWarshipBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SalmonWarshipBuff");
		// Description.SetDefault("提升速度\n“美味的三文鱼军舰”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxRunSpeed *= 1.1f;
	}
}
