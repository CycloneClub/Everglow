using Terraria.ModLoader.IO;

namespace Everglow.Plant.Common;

public class PlantModSystem : ModSystem
{
	public override void SaveWorldData(TagCompound tag)
	{
		tag.Add("RollingCactusHitCount", RollingCactusHitCount);
	}

	public override void LoadWorldData(TagCompound tag)
	{
		RollingCactusHitCount = tag.GetByte("RollingCactusHitCount");
	}

	public static byte RollingCactusHitCount;
}
