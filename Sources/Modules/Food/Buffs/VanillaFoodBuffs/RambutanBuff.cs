namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class RambutanBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		//DisplayName.SetDefault("RambutanBuff");
		//Description.SetDefault("免疫许多减益\n“提高免疫”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.buffImmune[20] = true;
		player.buffImmune[70] = true; //免疫中毒和毒液

		player.buffImmune[33] = true;
		player.buffImmune[36] = true;
		player.buffImmune[30] = true;
		player.buffImmune[20] = true;
		player.buffImmune[32] = true;
		player.buffImmune[31] = true;
		player.buffImmune[35] = true;
		player.buffImmune[23] = true;
		player.buffImmune[22] = true; //十字章一样的免疫

	}
}

