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
		bool isThirsty = Main.LocalPlayer.GetModPlayer<FoodModPlayer>().Thirstystate;
		return this.GetLocalization(isThirsty ? "Thirsty" : "NotThirsty").Value;
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
