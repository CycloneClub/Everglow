namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class EscargotBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("EscargotBuf");
		// Description.SetDefault("大大减速，加25%减伤\n“神龟药水？”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxRunSpeed *= 0.25f; // 减速
		player.desertBoots = false;
		player.wingTime /= 10;
		player.wingTimeMax /= 10;
		player.rocketTime /= 10;
		player.moveSpeed *= 0.25f;
		player.runAcceleration *= 0.25f;
		player.jumpSpeedBoost *= 0.25f;
		player.endurance += 0.25f; // 加25%减伤

	}
}

