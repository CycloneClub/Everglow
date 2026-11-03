namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChocolateChipCookieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 2; // 加2生命恢复
		// +2 mana regen
		player.manaRegenBonus += 2;
	}
}
