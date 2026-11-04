namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class LemonadeBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +100% ranged knockback
		player.GetKnockback(DamageClass.Ranged) += 1f;
		player.aggro -= 2400; // 仇恨值减2400
	}
}
