namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PlumBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.aggro += 600; // 加600仇恨值
		player.GetAttackSpeed(DamageClass.Generic) += 0.08f; // 加8%攻速
	}
}
