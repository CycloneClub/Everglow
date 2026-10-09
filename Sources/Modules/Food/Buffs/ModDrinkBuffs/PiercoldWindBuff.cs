namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class PiercoldWindBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PiercoldWindBuff");
		// Description.SetDefault("对灯笼月敌怪特攻\n“长啸出原野，凛然寒风生。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.PiercoldWindBuff = true;

	}
}

