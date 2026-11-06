namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PeachBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 1; // 加1生命回复
		player.lifeMagnet = true; // 增加心的拾取范围
	}
}
