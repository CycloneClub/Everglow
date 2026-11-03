namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SugarCookieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +5% ranged damage
		player.GetDamage(DamageClass.Ranged) += 0.05f;
	}
}
