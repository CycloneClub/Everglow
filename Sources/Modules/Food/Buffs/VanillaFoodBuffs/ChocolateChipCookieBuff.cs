namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChocolateChipCookieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ChocolateChipCookieBuff");
		// Description.SetDefault("增加2点生命回复和魔力回复\n“补充能量”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 2; // 加2生命恢复
		player.manaRegen += 2; // 加2魔力恢复

	}
}

