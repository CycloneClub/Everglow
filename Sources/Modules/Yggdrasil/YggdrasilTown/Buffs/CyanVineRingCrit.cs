namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class CyanVineRingCrit : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 5;
	}
}
