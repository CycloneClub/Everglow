namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class CoffeeCupBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CoffeeCupBuff");
		// Description.SetDefault("短时间内大幅增加铺墙铺砖速度，高亮标记敌人、陷阱和宝藏，并散发光芒\n“社畜的宝物”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.tileSpeed *= 10f;
		player.wallSpeed *= 10f;
		player.pickSpeed *= 0.1f;
		player.nightVision = true;
		player.findTreasure = true;
		player.detectCreature = true;
		player.dangerSense = true;
		Lighting.AddLight(player.Center, 0.8f, 0.8f, 0);
	}
}
