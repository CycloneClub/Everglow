namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SalmonInPepperBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SalmonInPepperBuff");
		// Description.SetDefault("提升速度,尤其是在水中\n“冰海之皇”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.accRunSpeed *= 1.07f;
		player.maxRunSpeed *= 1.07f;
		if (Main.raining)
		{
			player.accRunSpeed *= 1.15f;
			player.maxRunSpeed *= 1.15f;
		}
		player.accFlipper = true;
	}
}

