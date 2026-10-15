using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

public interface ITagCompoundEntity
{
	/// <summary>
	/// Allows you to save custom data for this player.
	/// <para/>NOTE: The provided tag is always empty by default, and is provided as an argument only for the sake of convenience and optimization.
	/// <para/>NOTE: Try to only save data that isn't default values.
	/// </summary>
	/// <param name="tag"></param>
	public void SaveData(TagCompound tag);

	/// <summary>
	/// Allows you to load custom data that you have saved for this player.
	/// <para/>Try to write defensive loading code that won't crash if something's missing.
	/// </summary>
	/// <param name="tag"></param>
	public void LoadData(TagCompound tag);
}
