namespace Everglow.Myth.LanternMoon.Buffs;

public class WhiteImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("White Flame Immunity");
		// Description.SetDefault("Spiritual Fiery Core's white flame cannot hurt you");
		// //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "花火幻魂心的白色火焰无法伤害到你");
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}
}
