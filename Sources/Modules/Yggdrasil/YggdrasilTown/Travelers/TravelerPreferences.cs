using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

/// <summary>Local presentation preferences; does not synchronize or enable travelers in multiplayer.</summary>
public class TravelerPreferences : ModPlayer
{
	public TravelerOptions Options { get; private set; } = new();

	public override void Initialize() => Options = new TravelerOptions();

	public override void SaveData(TagCompound tag) => tag["TownTravelerOptions"] = Options.Save();

	public override void LoadData(TagCompound tag)
	{
		Options = new TravelerOptions();
		if (tag.TryGet("TownTravelerOptions", out TagCompound settings))
		{
			Options.Load(settings);
		}
	}

	public override void OnEnterWorld()
	{
		if (!Main.dedServ && Player.whoAmI == Main.myPlayer)
		{
			TownTravelerSystem.Options = Options;
		}
	}
}
