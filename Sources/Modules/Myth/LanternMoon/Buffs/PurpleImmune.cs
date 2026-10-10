namespace Everglow.Myth.LanternMoon.Buffs;

public class PurpleImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Violet Flame Immunity");
		// Description.SetDefault("Spiritual Fiery Core's violet flame cannot hurt you");
		// //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "花火幻魂心的紫色火焰无法伤害到你");
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}
}
