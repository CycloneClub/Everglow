namespace Everglow.Food.Buffs.ModFoodBuffs;

public class TamakoSushiBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 2;
	}
}
