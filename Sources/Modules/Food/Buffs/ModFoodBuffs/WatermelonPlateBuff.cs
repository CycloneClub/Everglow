namespace Everglow.Food.Buffs.ModFoodBuffs;

public class WatermelonPlateBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("WatermelonPlateBuff");
		// Description.SetDefault("提升6%暴击率和爆伤\n“朴实无华的摆盘”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 6;
		FoodBuffModPlayer.AddCritDamage += 1.06f;
	}
}

