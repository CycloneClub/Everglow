namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class MarshmallowBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.hasJumpOption_Cloud = true;
	}
}
