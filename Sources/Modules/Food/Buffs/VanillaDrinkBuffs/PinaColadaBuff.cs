namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PinaColadaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.thorns += 10f; // 十倍反伤
	}
}
