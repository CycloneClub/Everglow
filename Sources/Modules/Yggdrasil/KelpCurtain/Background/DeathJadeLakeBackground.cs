using System.Runtime.InteropServices;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class DeathJadeLakeBackground : ModSystem
{
	public static Vector2 BiomeCenter = new Vector2(325, 18775) * 16;

	public float Alpha = 0f;

	public override void PostUpdateEverything()
	{
		const float increase = 0.02f;
		if (Main.LocalPlayer.InModBiome<DeathJadeLakeBiome>() && Main.BackgroundEnabled)
		{
			if (Alpha == 0)
			{
				DeathJadeLakeBiome.GetLiquidSurfaceY();
			}
			if (Alpha < 1)
			{
				Alpha += increase;
			}
			else
			{
				Alpha = 1;
			}
		}
		else
		{
			if (Alpha > 0)
			{
				Alpha -= increase;
			}
			else
			{
				Alpha = 0;
			}
		}
		if (Alpha > 0)
		{
			BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();

			DeathJadeLakeWater_Sky lake_sky = new DeathJadeLakeWater_Sky();
			lake_sky.WorldAnchor = BiomeCenter;
			bgSystem.AddBackgroundSlide(lake_sky);

			DeathJadeLakeWater_5 lake_5 = new DeathJadeLakeWater_5();
			lake_5.WorldAnchor = BiomeCenter;
			bgSystem.AddBackgroundSlide(lake_5);

			DeathJadeLakeWater_4 lake_4 = new DeathJadeLakeWater_4();
			lake_4.WorldAnchor = BiomeCenter;
			bgSystem.AddBackgroundSlide(lake_4);

			DeathJadeLakeWater_3 lake_3 = new DeathJadeLakeWater_3();
			lake_3.WorldAnchor = BiomeCenter;
			bgSystem.AddBackgroundSlide(lake_3);

			DeathJadeLakeWater_2 lake_2 = new DeathJadeLakeWater_2();
			lake_2.WorldAnchor = BiomeCenter;
			bgSystem.AddBackgroundSlide(lake_2);

			DeathJadeLakeWater_TyndallLight lake_TyndallLight = new DeathJadeLakeWater_TyndallLight();
			lake_TyndallLight.WorldAnchor = BiomeCenter;
			bgSystem.AddBackgroundSlide(lake_TyndallLight);
		}
	}

	/// <summary>
	/// Use high performance method to draw the background.
	/// </summary>
	/// <param name="bg"></param>
	public static void DrawBackground(BackgroundSlideBase bg)
	{
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
		var bars = new List<Vertex2D>();
		int yLayers = (int)((drawBottom - drawTop) / 16f);

		int currentSize = bars.Count;
		int estimatedCapacity = bars.Count + yLayers * 6 + 256;
		if (bars.Capacity < estimatedCapacity)
		{
			bars.Capacity = estimatedCapacity;
		}
		CollectionsMarshal.SetCount(bars, estimatedCapacity);
		Span<Vertex2D> span = CollectionsMarshal.AsSpan(bars);

		for (int offsetY = 0; offsetY < yLayers; offsetY++)
		{
			float rightClamp = Main.screenWidth + Main.offScreenRange + Main.screenPosition.X;
			float rightBound = Main.maxTilesX * 16;
			int tileY = (int)(drawTop / 16) + offsetY;
			if (DeathJadeLakeBiome.RightBoundOfACertainY.ContainsKey(tileY))
			{
				int rightX;
				DeathJadeLakeBiome.RightBoundOfACertainY.TryGetValue(tileY, out rightX);
				rightBound = rightX * 16;
			}
			if (rightClamp > rightBound)
			{
				rightClamp = rightBound;
			}
			float left = Main.screenPosition.X - Main.offScreenRange;
			float top = tileY * 16;
			BackgroundHigherPerformanceHelper.Add_WorldTriangle(bg, new Vector2(left, top), new Vector2(rightClamp, top), new Vector2(rightClamp, top + 16), span, ref currentSize);
			BackgroundHigherPerformanceHelper.Add_WorldTriangle(bg, new Vector2(left, top), new Vector2(left, top + 16), new Vector2(rightClamp, top + 16), span, ref currentSize);
		}
		CollectionsMarshal.SetCount(bars, currentSize);
		BackgroundSlideBase.DrawVertexBackground(bg, PrimitiveType.TriangleList, bars);
	}
}
