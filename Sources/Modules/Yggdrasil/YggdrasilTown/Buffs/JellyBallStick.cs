namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class JellyBallStick : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// -50% movement speed
		player.moveSpeed -= 0.5f;
		base.Update(player, ref buffIndex);
	}
}
