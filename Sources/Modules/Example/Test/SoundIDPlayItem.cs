using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Example.Test;

/// <summary>
/// Devs only.
/// </summary>
public class SoundIDPlayItem : ModItem
{
	public override void SetDefaults()
	{
		Item.useTime = 21;
		Item.useAnimation = 21;
	}

	public int soundID = 0;

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			soundID++;
			if (soundID >= SoundID.TrackableLegacySoundCount)
			{
				soundID = 0;
			}
			SoundEngine.PlaySound(soundID);
			string message = "SoundID = " + soundID;
			Main.NewText(message);
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			soundID--;
			if (soundID < 0)
			{
				soundID = SoundID.TrackableLegacySoundCount - 1;
			}
			SoundEngine.PlaySound(soundID);
			string message = "SoundID = " + soundID;
			Main.NewText(message);
		}
		if (Main.mouseMiddle && Main.mouseMiddleRelease)
		{
			soundID = 62;
			SoundEngine.PlaySound(soundID);
			string message = "SoundID = " + soundID;
			Main.NewText(message);
		}
	}
}
