namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BlackCurrantBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.nightVision = true; // 获得夜视能力
		player.dangerSense = true; // 获得危险感知
	}
}
