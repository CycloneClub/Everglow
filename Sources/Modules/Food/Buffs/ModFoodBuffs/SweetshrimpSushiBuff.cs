namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SweetshrimpSushiBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +4% damage
		player.GetDamage(DamageClass.Generic) += 0.04f;
	}
}
