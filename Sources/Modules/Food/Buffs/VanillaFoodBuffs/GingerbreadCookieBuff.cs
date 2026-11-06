namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GingerbreadCookieBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.resistCold = true; // 保暖
		player.lifeRegen += 2; // 加2生命回复
	}
}
