namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedFishBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		//DisplayName.SetDefault("CookedFishBuff");
		//Description.SetDefault("增加40魔力上限,4%魔法暴击率\n“益气健脾”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Magic) += 0.04f; //加8%魔法暴击率
		player.statManaMax2 += 40; //加40魔力上限

	}
}

