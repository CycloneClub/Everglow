namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedFishBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +4% magic critical strike chance
		player.GetCritChance(DamageClass.Magic) += 4f;
		// +40 mana
		player.statManaMax2 += 40;
	}
}
