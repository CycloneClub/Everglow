namespace Everglow.Food.Buffs.ModFoodBuffs;

public class MapoTofuBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 3;
	}
}
