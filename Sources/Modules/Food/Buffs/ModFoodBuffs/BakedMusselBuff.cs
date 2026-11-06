namespace Everglow.Food.Buffs.ModFoodBuffs;

public class BakedMusselBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer.AddCritDamage += 0.06f;
		player.statDefense += 4;
	}
}
