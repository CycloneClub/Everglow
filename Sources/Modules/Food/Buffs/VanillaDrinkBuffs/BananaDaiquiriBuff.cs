namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class BananaDaiquiriBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BananaDaiquiriBuff");
		// Description.SetDefault("短时间内不消耗子弹，大幅增加远程伤害与暴击\n“低体温血压”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BananaDaiquiriBuff = true;
		player.GetDamage(DamageClass.Ranged) *= 1.5f; // 加50%伤害
		player.GetCritChance(DamageClass.Ranged) += 50; // 加50%暴击
	}
}

