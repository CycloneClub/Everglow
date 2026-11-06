namespace Everglow.Myth.Misc.Buffs;

public class LaserWormBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}
}
