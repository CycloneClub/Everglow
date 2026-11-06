namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ShrimpPoBoyBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 6; // 加6防御
		player.pickSpeed -= 0.25f; // 加25%挖矿速度
	}
}
