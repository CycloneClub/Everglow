namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SpaghettiBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.maxTurrets += 1; // 增加1哨兵栏
	}
}
