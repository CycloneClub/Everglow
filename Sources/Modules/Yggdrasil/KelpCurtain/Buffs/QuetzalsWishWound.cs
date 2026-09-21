using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class QuetzalsWishWound : ModBuff
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Buffs;

	public override string Texture => $"Terraria/Images/Buff_{BuffID.Bleeding}";

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}
}
