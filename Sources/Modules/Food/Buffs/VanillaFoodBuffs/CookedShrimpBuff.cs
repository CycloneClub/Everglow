namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedShrimpBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CookedShrimpBuff");
		// Description.SetDefault("增加10防御,4点盔甲穿透\n“补钙”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 10; // 加10防御
		player.GetArmorPenetration(DamageClass.Generic) += 4; // 加4穿甲

	}
}

