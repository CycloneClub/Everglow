namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BurgerBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// -40% movement speed
		player.moveSpeed -= 0.4f;
		player.statDefense += 10;
	}
}
