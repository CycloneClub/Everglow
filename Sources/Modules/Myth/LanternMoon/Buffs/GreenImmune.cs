namespace Everglow.Myth.LanternMoon.Buffs;

public class GreenImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Green Flame Immunity");
		// Description.SetDefault("Spiritual Fiery Core's green flame cannot hurt you");
		// //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "花火幻魂心的绿色火焰无法伤害到你");
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}
}
