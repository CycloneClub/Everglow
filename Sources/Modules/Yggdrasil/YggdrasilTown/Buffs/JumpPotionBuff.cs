using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class JumpPotionBuff : ModBuff
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Buffs;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<FishingPotionPlayer>().JumpPotionActive = true;
	}
}
