using System.Collections.Generic;
using Terraria;
namespace Everglow.Myth.LanternMoon.LanternCommon;

public class LanternMoonInvasionGlobalNPC : GlobalNPC
{
	public LanternMoonInvasionEvent LanternMoon = ModContent.GetInstance<LanternMoonInvasionEvent>();

	public override bool InstancePerEntity => true;

	public override void EditSpawnPool(IDictionary<int, float> pool, NPC.Spawner spawner)
	{
		if (LanternMoon is not null && LanternMoon.Active)
		{
			foreach (int key in pool.Keys.ToList())
			{
				pool[key] = 0f;
			}
			if (LanternMoon.Active && LanternMoon.Wave > 0 && LanternMoon.WaveEnemiesType[LanternMoon.Wave - 1] is not null)
			{
				if (LanternMoon.Wave != 15)
				{
					foreach (var typeAndChance in LanternMoon.WaveEnemiesType[LanternMoon.Wave - 1])
					{
						int type = typeAndChance.Type;
						float rate = typeAndChance.Chance;
						pool.Add(type, rate);
					}
				}
			}
		}
		base.EditSpawnPool(pool, spawner);
	}

	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		if (LanternMoon is not null && LanternMoon.Active)
		{
			if (spawnRate > 0)
			{
				spawnRate /= 15;
				maxSpawns = 60;
			}
		}
		base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
	}
}
