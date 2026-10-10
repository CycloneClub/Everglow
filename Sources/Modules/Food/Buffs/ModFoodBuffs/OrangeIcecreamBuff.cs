namespace Everglow.Food.Buffs.ModFoodBuffs;

public class OrangeIcecreamBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("OrangeIcecreamBuff");
		// Description.SetDefault("延长无敌帧时间\n“真实存于心中”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.immuneTime += 30;
	}
}
