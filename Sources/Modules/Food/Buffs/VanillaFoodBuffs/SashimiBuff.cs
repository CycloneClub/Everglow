using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SashimiBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().Sashimi = true;
		player.gills = true;
		player.ignoreWater = true;
		player.accFlipper = true;
		// +20% movement speed
		player.moveSpeed += 0.2f;
		// +15% damage
		player.GetDamage(DamageClass.Generic) += 0.15f;
	}
}
