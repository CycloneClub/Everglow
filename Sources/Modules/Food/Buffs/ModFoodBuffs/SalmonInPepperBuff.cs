namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SalmonInPepperBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +7% movement speed
		player.moveSpeed += 0.07f;
		// While it is raining:
		if (Main.raining)
		{
			// +15% movement speed
			player.moveSpeed += 0.15f;
		}
		player.accFlipper = true;
	}
}
