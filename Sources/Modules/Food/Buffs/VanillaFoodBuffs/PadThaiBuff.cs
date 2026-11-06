namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PadThaiBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetKnockback(DamageClass.Generic) += 0.50f; // 加50%击退
	}
}
