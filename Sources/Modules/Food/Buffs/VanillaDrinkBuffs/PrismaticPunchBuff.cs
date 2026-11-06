namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PrismaticPunchBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.maxMinions += 5; // 加5召唤栏
		player.maxTurrets += 5; // 加5哨兵栏
		// +50% summon knockback
		player.GetKnockback(DamageClass.Summon) += 0.5f;
		// +50% summon damage
		player.GetDamage(DamageClass.Summon) += 0.5f;
	}
}
