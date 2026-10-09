namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class MilkCartonBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("MilkCarton Buff");
		// Description.SetDefault("短时间内免疫几乎所有debuff\n“一奶解百毒 ”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.buffImmune[30] = true;
		player.buffImmune[20] = true;
		player.buffImmune[24] = true;
		player.buffImmune[70] = true;
		player.buffImmune[22] = true;
		player.buffImmune[80] = true;
		player.buffImmune[35] = true;
		player.buffImmune[23] = true;
		player.buffImmune[31] = true;
		player.buffImmune[32] = true;
		player.buffImmune[197] = true;
		player.buffImmune[33] = true;
		player.buffImmune[36] = true;
		player.buffImmune[195] = true;
		player.buffImmune[196] = true;
		player.buffImmune[39] = true;
		player.buffImmune[69] = true;
		player.buffImmune[46] = true;
		player.buffImmune[47] = true;
		player.buffImmune[103] = true;
		player.buffImmune[149] = true;
		player.buffImmune[156] = true;
		player.buffImmune[164] = true;
		player.buffImmune[163] = true;
		player.buffImmune[144] = true;
		player.buffImmune[148] = true;
		player.buffImmune[145] = true;
		player.buffImmune[68] = true;
		player.buffImmune[67] = true;
		player.buffImmune[120] = true;
		player.buffImmune[44] = true;
		player.buffImmune[72] = true;
		player.buffImmune[137] = true;
		player.buffImmune[153] = true;
		player.buffImmune[204] = true;
		player.buffImmune[203] = true;
		player.buffImmune[169] = true;
		player.buffImmune[189] = true;
	}
}
