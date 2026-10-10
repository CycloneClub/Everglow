using Microsoft.Xna.Framework;

namespace Everglow.Food.InfoDisplays;

internal class ThirstystateInfoDisplay : InfoDisplay
{
	public override bool Active()
	{
		return Main.LocalPlayer.GetModPlayer<ThirstystateInfoDisplayplayer>().ShowThirstystate;
	}

	public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)/* tModPorter Suggestion: Set displayColor to InactiveInfoTextColor if your display value is "zero"/shows no valuable information */
	{
		bool Thirstystate = Main.LocalPlayer.GetModPlayer<FoodModPlayer>().Thirstystate;
		if (Thirstystate)
		{
			return Terraria.Localization.Language.GetTextValue("Mods.Everglow.InfoDisplay.Thirsty");
		}
		else
		{
			return Terraria.Localization.Language.GetTextValue("Mods.Everglow.InfoDisplay.NotThirsty");
		}
	}
}

public class ThirstystateInfoDisplayplayer : ModPlayer
{
	public bool AccOsmoticPressureMonitor;
	public bool ShowThirstystate;

	public override void ResetEffects()
	{
		AccOsmoticPressureMonitor = false;
		ShowThirstystate = false;
	}

	public override void UpdateEquips()
	{
		if (AccOsmoticPressureMonitor)
		{
			ShowThirstystate = true;
		}
	}
}
