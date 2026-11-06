namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PeachSangriaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 50; // 加50生命恢复
		player.lifeMagnet = true; // 增加心的拾取范围
	}
}
