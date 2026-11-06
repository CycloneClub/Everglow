namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class DryMartiniBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +100% damage
		player.GetDamage(DamageClass.Generic) += 1f;
	}
}
