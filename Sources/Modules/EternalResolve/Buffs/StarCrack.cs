namespace Everglow.EternalResolve.Buffs;

public class StarCrack : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.buffTime[buffIndex] += 1;
	}
}

public class StarCrackPlayer : ModPlayer
{
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Player.HasBuff(ModContent.BuffType<StarCrack>()))
		{
			modifiers.SetCrit();
			modifiers.CritDamage *= 2.64f;
			Player.ClearBuff(ModContent.BuffType<StarCrack>());
		}
		base.ModifyHitNPC(target, ref modifiers);
	}
}
