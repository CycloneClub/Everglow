using Everglow.Commons.FeatureFlags;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Common;

public class EternalResolveFishingPlayer : ModPlayer
{
	public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
	{
		bool inWater = !attempt.inLava && !attempt.inHoney;
		if (inWater && Player.ZoneBeach && attempt.veryrare)
		{
			itemDrop = ModContent.ItemType<SwordfishBeak>();
		}
	}
}
