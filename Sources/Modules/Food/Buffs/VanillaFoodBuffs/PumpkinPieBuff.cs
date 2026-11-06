namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PumpkinPieBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statLifeMax2 += 50;
	}
}
