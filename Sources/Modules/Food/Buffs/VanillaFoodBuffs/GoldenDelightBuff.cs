namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GoldenDelightBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GoldenDelightBuff");
		// Description.SetDefault("攻击造迈达斯，提升大部分属性\n“金灿灿!”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{

		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.GoldenDelightBuff = true;
		player.statDefense += 4; // 加3防御
		player.GetCritChance(DamageClass.Generic) += 4; // 加4%暴击
		player.GetDamage(DamageClass.Generic) *= 1.04f; // 加4%伤害
		player.GetAttackSpeed(DamageClass.Generic) += 0.04f; // 加4%攻速
		player.lifeRegen += 2; // 加2生命回复
		player.manaRegen += 4; // 魔力再生加4
		player.maxRunSpeed *= 1.2f; // 加速
		player.runAcceleration *= 1.2f;
		player.jumpSpeedBoost += 2;

	}
}

