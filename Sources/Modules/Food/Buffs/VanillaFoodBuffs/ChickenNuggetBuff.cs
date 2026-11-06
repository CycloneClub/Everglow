namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChickenNuggetBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 1; // 加1生命回复
		player.GetAttackSpeed(DamageClass.Generic) += 0.04f; // 加4%攻速
	}
}
