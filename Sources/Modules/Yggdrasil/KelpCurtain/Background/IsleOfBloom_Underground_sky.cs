using Everglow.Commons.Mechanics.EliminateLight;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class IsleOfBloom_Underground_sky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.IsleOfBloom_Underground_sky.Value;
		Distance = 2.8f;
		UseColorStyle = 2;
		LayerPriority = 2;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override void Draw()
	{
		Main.spriteBatch.Draw(Texture, WorldAnchor - Main.screenPosition, null, Color.White, 0, Texture.Size() * 0.5f, new Vector2(4, 1), SpriteEffects.None, 0);
		List<Vector2> polygon = new List<Vector2>();
		Vector2 centerPosWorld = YggdrasilWorld.KelpCurtain_IsleOfBloom_CaveCenter.ToWorldCoordinates() - new Vector2(0, 240);
		polygon.Add(centerPosWorld + new Vector2(-100, -40) * 16);
		polygon.Add(centerPosWorld + new Vector2(100, -40) * 16);
		polygon.Add(centerPosWorld + new Vector2(130, 30) * 16);
		polygon.Add(centerPosWorld + new Vector2(-130, 30) * 16);
		EliminateLightManager.AddPolygon(polygon);
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<IsleOfBloomBiome>();
	}
}
