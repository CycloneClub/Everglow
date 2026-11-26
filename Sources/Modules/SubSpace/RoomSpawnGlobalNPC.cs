using SubworldLibrary;

namespace Everglow.SubSpace;

public class RoomSpawnGlobalNPC : GlobalNPC
{
	public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		if (SubworldSystem.IsActive<RoomWorld>())
		{
			maxSpawns = 0;
		}
	}
}
