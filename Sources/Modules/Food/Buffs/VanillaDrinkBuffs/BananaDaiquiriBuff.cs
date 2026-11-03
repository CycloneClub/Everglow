namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class BananaDaiquiriBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BananaDaiquiriBuff = true;
		// +50% ranged damage
		player.GetDamage(DamageClass.Ranged) += 0.5f;
		player.GetCritChance(DamageClass.Ranged) += 50; // 加50%暴击
	}
}
