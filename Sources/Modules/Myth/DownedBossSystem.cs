using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Everglow.Myth;

// Acts as a container for "downed boss" flags.
// Set a flag like this in your bosses OnKill hook:
//    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

// Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
public class DownedBossSystem : ModSystem
{
	public static bool downedTusk = false;
	public static bool downedMoth = false;
	public static bool downedAcytaea = false;
	public static string TuskName;
	public static string MothName;
	public static string AcyName;
	// public static bool downedOtherBoss = false;
	public override void OnWorldLoad()
	{
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			TuskName = "鲜血獠牙";
			MothName = "腐檀巨蛾";
			AcyName = "雅思塔亚";
		}
		else
		{
			TuskName = "Bloody Tusk";
			MothName = "Corrupt Moth";
			AcyName = "Acytaea";
		}
		downedTusk = false;
		downedMoth = false;
		downedAcytaea = false;
		// downedOtherBoss = false;
	}

	public override void OnWorldUnload()
	{
		downedTusk = false;
		downedMoth = false;
		downedAcytaea = false;
		// downedOtherBoss = false;
	}

	// We save our data sets using TagCompounds.
	// NOTE: The tag instance provided here is always empty by default.
	public override void LoadWorldData(TagCompound tag)
	{
		downedTusk = tag.ContainsKey("downedTusk");
		downedMoth = tag.ContainsKey("downedMoth");
		downedAcytaea = tag.ContainsKey("downedAcytaea");
		// downedOtherBoss = tag.ContainsKey("downedOtherBoss");
	}
	public override void SaveWorldData(TagCompound tag)
	{
		if (downedTusk)
			tag["downedTusk"] = true;
		if (downedMoth)
			tag["downedMoth"] = true;
		if (downedAcytaea)
			tag["downedAcytaea"] = true;

		// if (downedOtherBoss) {
		// tag["downedOtherBoss"] = true;
		// }
	}
}
