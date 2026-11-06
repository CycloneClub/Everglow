namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SauteedFrogLegsBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.autoJump = true;
		player.jumpSpeedBoost += 1f;
		player.maxFallSpeed += 5f;
		player.jumpBoost = true;
	}
}
