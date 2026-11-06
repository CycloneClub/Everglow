namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SalmonWarshipBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +10% movement speed
		player.moveSpeed += 0.1f;
	}
}
