using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class DeathJadeLakeWater_TyndallLight : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = Commons.ModAsset.Noise_perlin.Value;
		Distance = 2.4f;
		LayerPriority = 1;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<DeathJadeLakeBiome>();
	}

	public override void Draw()
	{
		DeathJadeLakeBackground deathJadeLakeBackground = ModContent.GetInstance<DeathJadeLakeBackground>();
		if (deathJadeLakeBackground is null)
		{
			return;
		}
		Color baseColor = new Color(0.1f, 0.3f, 0.2f, 0f) * deathJadeLakeBackground.Alpha;
		float drawTop = DeathJadeLakeBiome.LiquidSurfaceY;
		if (drawTop - Main.screenPosition.Y < -Main.offScreenRange)
		{
			drawTop = -Main.offScreenRange + Main.screenPosition.Y;
		}
		float drawBottom = Main.screenPosition.Y + Main.screenHeight + Main.offScreenRange;
		if (drawTop > drawBottom)
		{
			return;
		}
		Vector2 totalOffset = (Main.screenPosition - WorldAnchor) * 1f / Distance / 300f;
		float timeValue = (float)(Main.time * 0.0005);

		var bars = new List<Vertex2D>();
		int yLayers = (int)((drawBottom - drawTop) / 16f);

		for (int offsetY = 0; offsetY < yLayers; offsetY++)
		{
			float rightClamp = Main.screenWidth + Main.offScreenRange;
			float rightBound = Main.maxTilesX * 16;
			int tileY = (int)(drawTop / 16) + offsetY;
			if (DeathJadeLakeBiome.RightBoundOfACertainY.ContainsKey(tileY))
			{
				int rightX;
				DeathJadeLakeBiome.RightBoundOfACertainY.TryGetValue(tileY, out rightX);
				rightBound = rightX * 16;
			}
			rightBound -= Main.screenPosition.X;
			if (rightClamp > rightBound)
			{
				rightClamp = rightBound;
			}
			float tilt = -0.01f;
			float tiltX0 = offsetY * tilt;
			float tileX1 = (offsetY + 1) * tilt;

			float fade = 1 - offsetY * 0.02f;
			if (fade <= 0)
			{
				break;
			}
			Color drawColor = baseColor * fade;
			float leftXcoord = totalOffset.X;
			float rightXcoord = totalOffset.X + (rightClamp + Main.offScreenRange) / Texture.Width;
			bars.Add(new Vector2(-Main.offScreenRange, drawTop + offsetY * 16 - Main.screenPosition.Y), drawColor, new Vector3(leftXcoord + tiltX0, timeValue, 0));
			bars.Add(new Vector2(rightClamp, drawTop + offsetY * 16 - Main.screenPosition.Y), drawColor, new Vector3(rightXcoord + tiltX0, timeValue, 0));
			bars.Add(new Vector2(-Main.offScreenRange, drawTop + (offsetY + 1) * 16 - Main.screenPosition.Y), drawColor, new Vector3(leftXcoord + tileX1, timeValue, 0));

			bars.Add(new Vector2(-Main.offScreenRange, drawTop + (offsetY + 1) * 16 - Main.screenPosition.Y), drawColor, new Vector3(leftXcoord + tileX1, timeValue, 0));
			bars.Add(new Vector2(rightClamp, drawTop + offsetY * 16 - Main.screenPosition.Y), drawColor, new Vector3(rightXcoord + tiltX0, timeValue, 0));
			bars.Add(new Vector2(rightClamp, drawTop + (offsetY + 1) * 16 - Main.screenPosition.Y), drawColor, new Vector3(rightXcoord + tileX1, timeValue, 0));
		}
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Texture;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		}
	}
}
