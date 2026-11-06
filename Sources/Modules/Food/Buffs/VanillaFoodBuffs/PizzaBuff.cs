namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PizzaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 10; // 加10穿甲
	}
}
