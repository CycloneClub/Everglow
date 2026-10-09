namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class LemonBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		//DisplayName.SetDefault("LemonBuff");
		//Description.SetDefault("加4%远程暴击,仇恨值减300\n“消炎美容”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Ranged) += 4; // 加4%暴击
		player.aggro -= 300; //仇恨值减300

	}
}

