namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class BloodyMoscatoBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BloodyMoscatoBuff");
		// Description.SetDefault("短时间内每次攻击回2点生命\n血色");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BloodyMoscatoBuff = true;
	}
}

