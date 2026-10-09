namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class FroggleBunwichBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("FroggleBunwichBuff");
		// Description.SetDefault("自动跳跃，增加伤害和跳跃能力\n“你能真正尝到沼泽的味道。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage(DamageClass.Generic) *= 1.04f; // 加4%伤害
		player.autoJump = true;
		player.jumpSpeedBoost += 1.5f;
		player.jumpBoost = true;
	}
}
