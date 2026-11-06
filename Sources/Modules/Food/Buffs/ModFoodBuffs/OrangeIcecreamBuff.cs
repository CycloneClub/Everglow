using Everglow.Commons.Mechanics.PlayerBonuses;

namespace Everglow.Food.Buffs.ModFoodBuffs;

public class OrangeIcecreamBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().OrangeIcecream = true;
	}
}
