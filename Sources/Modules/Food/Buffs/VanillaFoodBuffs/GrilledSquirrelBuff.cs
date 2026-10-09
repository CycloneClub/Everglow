namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrilledSquirrelBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GrilledSquirrelBuff");
		// Description.SetDefault("增加跳跃能力\n“欢跃”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.jumpSpeedBoost += 2;
		player.jumpBoost = true;
		player.maxFallSpeed *= 0.75f;
		player.extraFall += 30;

	}
}

