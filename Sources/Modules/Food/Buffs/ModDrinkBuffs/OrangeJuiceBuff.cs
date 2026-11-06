using Everglow.Commons.Mechanics.PlayerBonuses;

namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class OrangeJuiceBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().OrangeJuice = true;
	}
}
