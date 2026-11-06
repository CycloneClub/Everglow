namespace Everglow.Myth.LanternMoon.Buffs;

public class PurpleImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
