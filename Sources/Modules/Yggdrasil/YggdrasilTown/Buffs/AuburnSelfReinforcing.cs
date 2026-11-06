using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class AuburnSelfReinforcing : ModBuff
{
	public override string Texture => Commons.ModAsset.BuffTemplate_Mod;

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().AuburnSelfReinforcing = true;
		player.maxMinions += 1;
	}
}
