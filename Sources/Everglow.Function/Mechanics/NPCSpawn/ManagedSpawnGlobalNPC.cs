namespace Everglow.Commons.Mechanics.NPCSpawn;

/// <summary>把区域表和公共频率策略接入 tML；不记录实体归属或发送额外网络数据。</summary>
public sealed class ManagedSpawnGlobalNPC : GlobalNPC
{
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) =>
		ModContent.GetInstance<SubworldSpawnSystem>().EditSpawnPool(pool, spawnInfo);

	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) =>
		ModContent.GetInstance<SubworldSpawnSystem>().EditSpawnRate(player, ref spawnRate, ref maxSpawns);

	public override void SpawnNPC(int npc, int tileX, int tileY)
	{
		NPC first = Main.npc[npc];
		var group = ModContent.GetInstance<SubworldSpawnSystem>().TakeSpawnGroup(first.netID, tileX, tileY);
		if (group != null)
		{
			SpawnGroup.SpawnMembers(group, first);
		}
	}
}
