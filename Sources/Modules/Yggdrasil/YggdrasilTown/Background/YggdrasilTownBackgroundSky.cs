using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class YggdrasilTownBackgroundSky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.YggdrasilTownBackgroundSky.Value;
		Distance = float.PositiveInfinity;
	}

	public override bool CanActive()
	{
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		return YggdrasilWorld.InYggdrasil && YggdrasilTownBiome.IsTownLayer(screenCenter);
	}
}
