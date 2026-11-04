namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class MilkshakeBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +400% movement speed
		player.moveSpeed += 4f;
	}
}
