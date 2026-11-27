using ModLiquidLib.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Liquids;

public class JellyBallNutritionLiquid : ModLiquid
{
	public override void SetStaticDefaults()
	{
		// 取原版水（0.5）和蜂蜜（0.25）的移动倍率中点。
		const float movementMultiplier = 0.375f;
		PlayerMovementMultiplier = movementMultiplier;
		StopWatchMPHMultiplier = movementMultiplier;
		NPCMovementMultiplierDefault = movementMultiplier;
		ProjectileMovementMultiplier = movementMultiplier;

		SplashDustType = DustID.Water;
		SplashSound = SoundID.SplashWeak;
		AddMapEntry(new Color(95, 181, 235));
	}

	public override int ChooseWaterfallStyle(int i, int j) => ModContent.GetInstance<JellyBallNutritionWaterfall>().Slot;
}
