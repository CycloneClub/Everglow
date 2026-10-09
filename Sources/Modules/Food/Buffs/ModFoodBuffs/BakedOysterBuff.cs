namespace Everglow.Food.Buffs.ModFoodBuffs;

public class BakedOysterBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BakedOysterBuff");
		// Description.SetDefault("增加4点盔甲穿透、4防御\n“耍酒疯”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 4;
		player.statDefense += 4;
	}
}

