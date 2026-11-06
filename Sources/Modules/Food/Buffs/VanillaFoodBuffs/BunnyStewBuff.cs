namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BunnyStewBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.autoJump = true;
		player.jumpSpeedBoost += 1.5f;
		player.jumpBoost = true;
	}
}
