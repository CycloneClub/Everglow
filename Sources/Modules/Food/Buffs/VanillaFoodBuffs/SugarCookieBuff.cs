namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SugarCookieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SugarCookieBuff");
		// Description.SetDefault("加5%远程伤害\n“养胃滋润”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage(DamageClass.Ranged) *= 1.05f; // 加5%伤害
	}
}
