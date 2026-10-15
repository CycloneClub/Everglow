using SubworldLibrary;

namespace Everglow.Yggdrasil;

public class YggdrasilWorldSystem : ModSystem
{
	public override void Load()
	{
		On_NPC.AI_007_FindGoodRestingSpot += On_hook_AI_007_FindGoodRestingSpot;
		On_NPC.AI_007_TryForcingSitting += On_hook_AI_007_TryForcingSitting;
		base.Load();
	}

	public void On_hook_AI_007_FindGoodRestingSpot(On_NPC.orig_AI_007_FindGoodRestingSpot orig, NPC self, int myTileX, int myTileY, out int floorX, out int floorY)
	{
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			floorX = -1;
			floorY = -1;
			orig(self, myTileX, myTileY, out floorX, out floorY);
			return;
		}
		orig(self, myTileX, myTileY, out floorX, out floorY);
	}

	public void On_hook_AI_007_TryForcingSitting(On_NPC.orig_AI_007_TryForcingSitting orig, NPC self, int floorX, int floorY)
	{
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			if (floorX > 0 && floorX < Main.tile.Width && floorY > 0 && floorY < Main.tile.Height)
			{
				orig(self, floorX, floorY);
			}
			return;
		}
		orig(self, floorX, floorY);
	}

	public override void PostUpdateEverything()
	{
		if (YggdrasilWorld.InYggdrasil)
		{
			YggdrasilWorld.YggdrasilTimer++;

			if (Main.bloodMoon)
			{
				Main.bloodMoon = false;
			}
			if (Main.slimeRain)
			{
				Main.slimeRain = false;
			}
		}
		base.PostUpdateEverything();
	}
}
