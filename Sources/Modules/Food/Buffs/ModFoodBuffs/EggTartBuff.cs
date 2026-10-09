namespace Everglow.Food.Buffs.ModFoodBuffs;

public class EggTartBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("EggTartBuff");
		// Description.SetDefault("增加5%爆伤\n“这酥皮不错”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer.AddCritDamage += 1.05f;
	}
}
