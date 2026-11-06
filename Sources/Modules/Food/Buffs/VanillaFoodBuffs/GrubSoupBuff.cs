using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrubSoupBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().GrubSoup = true;
		player.fishingSkill += 30;
	}
}
