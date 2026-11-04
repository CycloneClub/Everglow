namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BurgerBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// -40% movement speed
		player.moveSpeed -= 0.4f;
		player.statDefense += 10;
	}
}
