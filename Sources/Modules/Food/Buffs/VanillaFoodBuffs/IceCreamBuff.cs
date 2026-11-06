namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class IceCreamBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.buffImmune[BuffID.OnFire] = true; // 免疫着火
		player.fireWalk = true; // 免疫火块
	}
}
