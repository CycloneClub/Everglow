namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class GrapeJuiceBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +10000 luck
		player.equipmentBasedLuckBonus += 10000f;
	}
}
