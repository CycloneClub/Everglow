namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PeachSangriaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PeachSangriaBuff");
		// Description.SetDefault("短时间内大幅回复生命，增加心的拾取范围\n“我也是桃饱用户”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 50; // 加50生命恢复 
		player.lifeMagnet = true; // 增加心的拾取范围
	}
}

