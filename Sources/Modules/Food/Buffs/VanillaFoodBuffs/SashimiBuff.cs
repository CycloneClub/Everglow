namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SashimiBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SashimiBuff");
		// Description.SetDefault("可以游泳，水下呼吸，增加15%伤害，但每秒减少2点生命\n“寄生虫！”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.SashimiBuff = true;
		player.gills = true;
		player.ignoreWater = true;
		player.accFlipper = true;
		player.maxRunSpeed *= 1.2f;
		player.GetDamage(DamageClass.Generic) *= 1.15f;

	}
}

