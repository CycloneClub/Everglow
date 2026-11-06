namespace Everglow.Myth.LanternMoon.Buffs;

public class BrownImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
