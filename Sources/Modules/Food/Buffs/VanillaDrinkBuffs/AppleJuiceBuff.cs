namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class AppleJuiceBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.endurance += 0.6f; // 加80%减伤
	}
}
