namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class EscargotBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// -75% movement speed
		player.moveSpeed -= 0.75f;
		player.desertBoots = false;
		// -75% final jump speed
		player.GetModPlayer<FoodBuffModPlayer>().EscargotBuff = true;
		player.endurance += 0.25f; // 加25%减伤
	}
}
