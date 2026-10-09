namespace Everglow.Food.Buffs.ModFoodBuffs;

public class CrabseedWarshipBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CrabseedWarshipBuff");
		// Description.SetDefault("提升暴击率\n“美味的蟹籽军舰”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 4; // 加4暴击
	}
}

