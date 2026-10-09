namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SeafoodPizzaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SeafoodPizzaBuff");
		// Description.SetDefault("提升大部分属性\n“一个人吃完这个可不容易”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 8; // 加8%暴击
		player.GetDamage(DamageClass.Generic) *= 1.08f; // 加8%伤害
		player.GetAttackSpeed(DamageClass.Generic) *= 1.08f; // 加8%攻速
		FoodBuffModPlayer.AddCritDamage += 0.08f;
		player.statDefense += 2; // 加2防御
		player.lifeRegen += 1; // 加1生命回复
		player.manaRegen += 1; // 魔力再生加1

	}
}

