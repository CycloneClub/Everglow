namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class FroggleBunwichBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +4% damage
		player.GetDamage(DamageClass.Generic) += 0.04f;
		player.autoJump = true;
		player.jumpSpeedBoost += 1.5f;
		player.jumpBoost = true;
	}
}
