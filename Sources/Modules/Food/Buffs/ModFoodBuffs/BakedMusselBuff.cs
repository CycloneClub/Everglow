namespace Everglow.Food.Buffs.ModFoodBuffs;

public class BakedMusselBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BakedMusselBuff");
		// Description.SetDefault("增加6%爆伤、4防御\n“似乎没那么生猛了”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer.AddCritDamage += 0.06f;
		player.statDefense += 4;
	}
}
