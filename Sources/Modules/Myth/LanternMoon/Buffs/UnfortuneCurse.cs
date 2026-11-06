namespace Everglow.Myth.LanternMoon.Buffs;

public class UnfortuneCurse : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// -100 luck
		player.equipmentBasedLuckBonus -= 100f;
		player.GetCritChance(DamageClass.Generic) -= 1f;
		base.Update(player, ref buffIndex);
	}
}
