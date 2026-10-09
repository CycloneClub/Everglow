namespace Everglow.Food.Buffs.ModFoodBuffs;

public class CaramelPuddingBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CaramelPuddingBuff");
		// Description.SetDefault("射弹可以多反弹一次\n“duangduangduang”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.CaramelPuddingBuff = true;
	}
}

