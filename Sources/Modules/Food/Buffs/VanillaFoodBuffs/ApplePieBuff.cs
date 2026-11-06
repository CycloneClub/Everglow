namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ApplePieBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.endurance += 0.05f; // 加8%减伤
		player.lifeRegen += 1;
	}
}
