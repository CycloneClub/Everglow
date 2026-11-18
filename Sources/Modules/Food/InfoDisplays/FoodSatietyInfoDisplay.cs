using Microsoft.Xna.Framework;

namespace Everglow.Food.InfoDisplays;

public class FoodSatietyInfoDisplay : InfoDisplay
{
	public override bool Active()
	{
		return Main.LocalPlayer.GetModPlayer<FoodSatietyInfoDisplayplayer>().ShowCurrentSatiety;
	}

	public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)/* tModPorter Suggestion: Set displayColor to InactiveInfoTextColor if your display value is "zero"/shows no valuable information */
	{
		int currentSatiety = Main.LocalPlayer.GetModPlayer<FoodModPlayer>().CurrentSatiety;
		int level = Main.LocalPlayer.GetModPlayer<FoodModPlayer>().SatietyLevel;
		string levelText = this.GetLocalization($"SatietyLevel.{level}").Value;
		return this.GetLocalization("Satiety").Format(currentSatiety, levelText);
	}
}

public class FoodSatietyInfoDisplayplayer : ModPlayer
{
	public bool AccBloodGlucoseMonitor;
	public bool ShowCurrentSatiety;

	public override void ResetEffects()
	{
		AccBloodGlucoseMonitor = false;
		ShowCurrentSatiety = false;
	}

	public override void UpdateEquips()
	{
		if (AccBloodGlucoseMonitor)
		{
			ShowCurrentSatiety = true;
		}
	}
}
