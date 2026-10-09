using Everglow.Commons.FeatureFlags;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Common;

public class EternalResolveFishingPlayer : ModPlayer
{
	internal FishingAttempt fishAttempt;

	public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
	{
		bool inWater = !fishAttempt.inLava && !fishAttempt.inHoney;
		if (inWater && Player.ZoneBeach && fishAttempt.veryrare)
		{
			fishAttempt.rolledItemDrop = ModContent.ItemType<SwordfishBeak>();

			if (EverglowConfig.DebugMode)
			{ Main.NewText("CatchFish rolledItemDrop: " + fishAttempt.rolledItemDrop); }

			itemDrop = ModContent.ItemType<SwordfishBeak>();

			if (EverglowConfig.DebugMode)
			{ Main.NewText("CatchFish itemDrop: " + fishAttempt.rolledItemDrop); }

			// sonar.Text = "Swordfish Beak";
			// sonar.Color = Color.AliceBlue;
			// sonar.Velocity = Vector2.Zero;
			// sonar.DurationInFrames = 300;
		}
	}

	public override bool? CanConsumeBait(Item bait)
	{
		PlayerFishingConditions conditions = Player.GetFishingConditions();

		if (EverglowConfig.DebugMode)
		{ Main.NewText("CanConsumeBait: " + fishAttempt.rolledItemDrop); }

		// The golden fishing rod will never consume bait
		if (fishAttempt.rolledItemDrop == ModContent.ItemType<SwordfishBeak>() && conditions.Pole.type == ItemID.GoldenFishingRod)
		{
			return false;
		}

		return null;
	}
}
