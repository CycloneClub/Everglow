using Everglow.Commons.Vertex;
using Terraria.UI;

namespace Everglow.Food.UI;

public class StoveUIManager : GameInterfaceLayer
{
	public static List<PotUI> PotUIs = new List<PotUI>();

	public StoveUIManager(string name, InterfaceScaleType scaleType)
		: base(name, scaleType)
	{
	}

	public override bool DrawSelf()
	{
		if (!Main.gamePaused)
		{
			Update();
		}
		foreach (var potUI in PotUIs)
		{
			potUI.Update();
			potUI.Draw();
		}
		return true;
	}

	public void Update()
	{
		for (int index = PotUIs.Count - 1; index >= 0; index--)
		{
			if (!PotUIs[index].Open)
			{
				int foodCount = 0;
				for (int i = 0; i < 6; i++)
				{
					if (PotUIs[index].Ingredients[i] != -1)
					{
						foodCount++;
					}
				}
				if (foodCount == 0)
				{
					PotUIs.RemoveAt(index);
				}
			}
		}
	}

	/// <summary>
	/// Width is half width, height as well.
	/// </summary>
	/// <param name="anchorCenter"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="color"></param>
	/// <param name="alpha">Fade amount subtracted from one to scale the panel color; one makes the panel transparent.</param>
	/// <param name="texture">Nine-piece panel texture; null uses the default stove UI panel texture.</param>
	public static void Draw9Pieces(Vector2 anchorCenter, float width, float height, Color color, float alpha, Texture2D texture = default)
	{
		color *= 1 - alpha;
		var bars = new List<Vertex2D>();

		if (width > 10 && height > 10)
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(10, 10), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(-10, 10), new Vector2(0.5f, 0), new Vector2(0.5f, 0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, 10), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(0, 10), anchorCenter + new Vector2(-width, height) + new Vector2(10, -10), new Vector2(0, 0.4f), new Vector2(0.2f, 0.6f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(10, 10), anchorCenter + new Vector2(width, height) + new Vector2(-10, -10), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-10, 10), anchorCenter + new Vector2(width, height) + new Vector2(0, -10), new Vector2(0.8f, 0.2f), new Vector2(1f, 0.8f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -10), anchorCenter + new Vector2(-width, height) + new Vector2(10, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(10, -10), anchorCenter + new Vector2(width, height) + new Vector2(-10, 0), new Vector2(0.5f, 0.8f), new Vector2(0.5f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-10, -10), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		else if (width < 10 && height > 10)
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(width, 10), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-width, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, 10), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(0, 10), anchorCenter + new Vector2(-width, height) + new Vector2(width, -10), new Vector2(0, 0.4f), new Vector2(0.2f, 0.6f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-width, 10), anchorCenter + new Vector2(width, height) + new Vector2(0, -10), new Vector2(0.8f, 0.2f), new Vector2(1f, 0.8f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -10), anchorCenter + new Vector2(-width, height) + new Vector2(width, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-width, -10), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		else if (width > 10 && height < 10)
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(10, height), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(-10, height), new Vector2(0.5f, 0), new Vector2(0.5f, 0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, height), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -height), anchorCenter + new Vector2(-width, height) + new Vector2(10, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(10, -height), anchorCenter + new Vector2(width, height) + new Vector2(-10, 0), new Vector2(0.5f, 0.8f), new Vector2(0.5f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-10, -height), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		else
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(width, height), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-width, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, height), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -height), anchorCenter + new Vector2(-width, height) + new Vector2(width, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-width, -height), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		if (texture == default)
		{
			texture = ModAsset.StoveUIPanel.Value;
		}
		Main.graphics.graphicsDevice.Textures[0] = texture;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
	}

	public static void AddRectangleBars(List<Vertex2D> bars, Vector2 posTopLeft, Vector2 posBottomRight, Vector2 coordTopLeft, Vector2 coordBottomRight, Color color)
	{
		bars.Add(posTopLeft, color, new Vector3(coordTopLeft, 0));
		bars.Add(new Vector2(posBottomRight.X, posTopLeft.Y), color, new Vector3(coordBottomRight.X, coordTopLeft.Y, 0));
		bars.Add(new Vector2(posTopLeft.X, posBottomRight.Y), color, new Vector3(coordTopLeft.X, coordBottomRight.Y, 0));

		bars.Add(new Vector2(posBottomRight.X, posTopLeft.Y), color, new Vector3(coordBottomRight.X, coordTopLeft.Y, 0));
		bars.Add(posBottomRight, color, new Vector3(coordBottomRight, 0));
		bars.Add(new Vector2(posTopLeft.X, posBottomRight.Y), color, new Vector3(coordTopLeft.X, coordBottomRight.Y, 0));
	}
}
