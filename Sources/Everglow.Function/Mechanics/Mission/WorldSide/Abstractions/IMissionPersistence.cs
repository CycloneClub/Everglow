using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public interface IMissionPersistence
{
	public void SaveData(TagCompound tag);

	public void LoadData(TagCompound tag);
}
