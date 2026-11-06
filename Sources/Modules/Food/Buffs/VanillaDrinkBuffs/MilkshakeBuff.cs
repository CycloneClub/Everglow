namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class MilkshakeBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +400% movement speed
		player.moveSpeed += 4f;
	}
}
