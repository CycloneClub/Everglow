namespace Everglow.Food.Buffs.ModFoodBuffs;

public class CantaloupeJellyBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CantaloupeJellyBuff");
		// Description.SetDefault("射弹可以多穿透一次\n“duangduangduang”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.CantaloupeJellyBuff = true;

	}
}

