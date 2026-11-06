namespace Everglow.Food.Buffs.ModFoodBuffs;

public class ArcticshellfishWarshipBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
	}
}
