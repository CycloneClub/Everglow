namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class LobsterTailBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 6; // 加6防御
		player.pickSpeed -= 0.33f; // 加25%挖矿速度
	}
}
