namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BloodOrangeBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statLifeMax2 += 25; // 加25血量上限
	}
}
