namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChickenNuggetBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ChickenNuggetBuff");
		// Description.SetDefault("增加1生命回复、4%攻速\n“数一数二的鸡块！”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 1; // 加1生命回复
		player.GetAttackSpeed(DamageClass.Generic) += 0.04f; // 加4%攻速

	}
}

