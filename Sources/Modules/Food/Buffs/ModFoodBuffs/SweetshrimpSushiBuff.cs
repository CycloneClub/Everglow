namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SweetshrimpSushiBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +4% damage
		player.GetDamage(DamageClass.Generic) += 0.04f;
	}
}
