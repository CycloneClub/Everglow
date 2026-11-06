namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PinaColadaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.thorns += 10f; // 十倍反伤
	}
}
