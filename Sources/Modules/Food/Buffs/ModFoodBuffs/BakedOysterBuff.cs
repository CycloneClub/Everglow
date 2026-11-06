namespace Everglow.Food.Buffs.ModFoodBuffs;

public class BakedOysterBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 4;
		player.statDefense += 4;
	}
}
