namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BlackCurrantBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BlackCurrantBuff");
		// Description.SetDefault("获得夜视、危险感知能力\n“改善視力” ");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.nightVision = true; // 获得夜视能力
		player.dangerSense = true; // 获得危险感知
	}
}
