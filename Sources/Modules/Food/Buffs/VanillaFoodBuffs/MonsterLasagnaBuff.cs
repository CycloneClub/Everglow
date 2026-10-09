namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class MonsterLasagnaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("MonsterLasagnaBuff");
		// Description.SetDefault("增加25%暴击率，但每秒减3生命\n“弄熟的恶魔还是恶魔，在千层饼中也不会改变。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.MonsterLasagnaBuff = true;
		player.GetCritChance(DamageClass.Generic) += 25;
	}
}

