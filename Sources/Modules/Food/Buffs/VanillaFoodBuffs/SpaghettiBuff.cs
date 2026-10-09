namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SpaghettiBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SpaghettiBuff");
		// Description.SetDefault("增加1哨兵栏\n“异域风情”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxTurrets += 1; // 增加1哨兵栏
	}
}

