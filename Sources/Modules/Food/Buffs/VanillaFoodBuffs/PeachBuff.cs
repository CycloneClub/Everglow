namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PeachBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PeachBuff");
		// Description.SetDefault("增加心的拾取范围，1生命回复\n“在想peach”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 1; // 加1生命回复
		player.lifeMagnet = true; // 增加心的拾取范围
	}
}

