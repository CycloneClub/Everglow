namespace Everglow.Food.Buffs.ModFoodBuffs;

public class TamakoSushiBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("TamakoSushiBuff");
		// Description.SetDefault("提升生命回复\n“美味的玉子寿司”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 2;
	}
}
