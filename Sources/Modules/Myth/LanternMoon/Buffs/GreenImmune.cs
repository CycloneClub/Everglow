namespace Everglow.Myth.LanternMoon.Buffs;

public class GreenImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
