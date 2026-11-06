namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class BananaDaiquiriBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BananaDaiquiriBuff = true;
		// +50% ranged damage
		player.GetDamage(DamageClass.Ranged) += 0.5f;
		player.GetCritChance(DamageClass.Ranged) += 50; // 加50%暴击
	}
}
