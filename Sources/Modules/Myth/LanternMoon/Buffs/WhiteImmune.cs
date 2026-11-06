namespace Everglow.Myth.LanternMoon.Buffs;

public class WhiteImmune : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
