namespace Everglow.Myth.LanternMoon.Buffs;

public class RedImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
