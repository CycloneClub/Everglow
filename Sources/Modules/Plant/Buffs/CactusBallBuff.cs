using Everglow.Plant.Common;

namespace Everglow.Plant.Buffs;

public class CactusBallBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		npc.defense = npc.defDefense - 12;
		if (npc.lifeRegen > 0)
		{
			npc.lifeRegen = 0;
		}

		npc.lifeRegen -= 5;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense -= 12;
		if (player.lifeRegen > 0)
		{
			player.lifeRegen = 0;
		}

		player.lifeRegen -= 5;
	}
}
