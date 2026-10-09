namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class DryMartiniBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("DryMartiniBuff");
		// Description.SetDefault("短时间大幅提升攻击\n“经典之作”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage(DamageClass.Generic) *= 2;
	}
}

