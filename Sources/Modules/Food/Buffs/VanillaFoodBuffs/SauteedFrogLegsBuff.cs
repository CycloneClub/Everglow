namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SauteedFrogLegsBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SauteedFrogLegsBuff");
		// Description.SetDefault("自动跳跃，提升跳跃能力\n“钓不到蛙腿？那就吃吧。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.autoJump = true;
		player.jumpSpeedBoost += 1f;
		player.maxFallSpeed += 5f;
		player.jumpBoost = true;
	}
}
