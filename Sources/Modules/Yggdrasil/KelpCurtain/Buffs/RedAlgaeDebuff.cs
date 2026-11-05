using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class RedAlgaeDebuff : ModBuff
{
	public const int LifeReduce = 12;

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().RedAlgaeLifeRegenLoss = LifeReduce;
	}
}
