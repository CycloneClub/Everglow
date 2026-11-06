namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BBQRibsBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statLifeMax2 += 50; // 加50血量上限
	}
}
