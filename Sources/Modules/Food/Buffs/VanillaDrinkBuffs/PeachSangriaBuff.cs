namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PeachSangriaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 50; // 加50生命恢复
		player.lifeMagnet = true; // 增加心的拾取范围
	}
}
