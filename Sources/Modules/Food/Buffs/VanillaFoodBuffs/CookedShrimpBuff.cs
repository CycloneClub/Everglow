namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedShrimpBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 10; // 加10防御
		player.GetArmorPenetration(DamageClass.Generic) += 4; // 加4穿甲
	}
}
