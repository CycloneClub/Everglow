using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class Wither_Activated_Dog_Buff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
	}
}
