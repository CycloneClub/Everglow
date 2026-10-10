using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using Terraria.Graphics.Light;

namespace Everglow.Commons.VFX;

public class VisualQualityController : IVisualQualityController
{
	public VisualQuality Quality => Lighting.Mode switch
	{
		LightMode.Color or LightMode.White => VisualQuality.High,
		_ => VisualQuality.Low,
	};
}
