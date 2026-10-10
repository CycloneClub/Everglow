using Everglow.Commons.Modules;

namespace Everglow.Food;

internal class FoodSystem : EverglowModule
{
	public override string Name => "Food";

	public override void Load()
	{
		On_Player.UpdateStarvingState += Player_UpdateStarvingState;
	}

	private void Player_UpdateStarvingState(Terraria.On_Player.orig_UpdateStarvingState orig, Player self, bool withEmote)
	{
		orig(self, false);
	}
}
