namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChristmasPuddingBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.aggro -= 800; // 仇恨值减800
	}
}
