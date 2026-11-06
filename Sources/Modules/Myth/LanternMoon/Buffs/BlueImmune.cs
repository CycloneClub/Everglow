namespace Everglow.Myth.LanternMoon.Buffs;

public class BlueImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
