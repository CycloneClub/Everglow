namespace Everglow.Food.Buffs.ModFoodBuffs;

public class KiwiIceCreamBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("KiwiIceCreamBuff");
		// Description.SetDefault("增大10%武器大小，加快武器攻速\n“奇异的力量”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.KiwiIceCreamBuff = true;
		player.GetAttackSpeed(DamageClass.Summon) *= 1.05f;
	}
}
