namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PineappleBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
		player.thorns += 0.5f; // 50%反伤
	}
}
