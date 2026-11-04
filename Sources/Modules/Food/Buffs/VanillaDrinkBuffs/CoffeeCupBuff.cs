namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class CoffeeCupBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// +900% tile placement speed
		player.tileSpeed += 9f;
		// +900% wall placement speed
		player.wallSpeed += 9f;
		// -90% mining time
		player.pickSpeed -= 0.9f;
		player.nightVision = true;
		player.findTreasure = true;
		player.detectCreature = true;
		player.dangerSense = true;
		Lighting.AddLight(player.Center, 0.8f, 0.8f, 0);
	}
}
