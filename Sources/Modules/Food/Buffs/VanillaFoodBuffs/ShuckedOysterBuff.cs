namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ShuckedOysterBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ShuckedOysterBuff");
		// Description.SetDefault("减少5防御，加10点盔甲穿透，每秒减少3点生命\n“小心寄生虫！”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.ShuckedOysterBuff = true;
		player.GetArmorPenetration(DamageClass.Generic) += 10;
		player.statDefense -= 5;
	}
}
