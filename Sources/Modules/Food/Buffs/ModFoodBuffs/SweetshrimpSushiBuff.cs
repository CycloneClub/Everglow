namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SweetshrimpSushiBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SweetshrimpSushiBuff");
		// Description.SetDefault("提升攻击\n“美味的甜虾寿司”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage(DamageClass.Generic) *= 1.04f;
	}
}

