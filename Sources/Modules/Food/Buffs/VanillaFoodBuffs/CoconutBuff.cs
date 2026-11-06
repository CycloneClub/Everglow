namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CoconutBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
		player.endurance += 0.03f; // 加3%减伤
	}
}
