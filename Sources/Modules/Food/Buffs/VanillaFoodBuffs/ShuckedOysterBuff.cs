using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ShuckedOysterBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().ShuckedOyster = true;
		player.GetArmorPenetration(DamageClass.Generic) += 10;
		player.statDefense -= 5;
	}
}
