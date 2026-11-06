namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class LobsterTailBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 6; // 加6防御
		player.pickSpeed -= 0.33f; // 加25%挖矿速度
	}
}
