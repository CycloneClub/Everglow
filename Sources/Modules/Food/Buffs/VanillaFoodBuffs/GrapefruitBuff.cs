namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrapefruitBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetKnockback(DamageClass.Summon) += 0.25f; // 加25%召唤物击退
	}
}
