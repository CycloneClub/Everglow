namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class AmberStick : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
	}
}

public class AmberStickNPC : GlobalNPC
{
	public override void PostAI(NPC npc)
	{
		if (npc.HasBuff(ModContent.BuffType<AmberStick>()))
		{
			npc.position -= npc.velocity * 0.9f;
		}
		base.PostAI(npc);
	}
}
