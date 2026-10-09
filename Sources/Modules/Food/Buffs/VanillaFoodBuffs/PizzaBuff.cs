namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PizzaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PizzaBuff");
		// Description.SetDefault("增加10点盔甲穿透\n“会让意大利人破防的菠萝披萨”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 10; // 加10穿甲
	}
}

