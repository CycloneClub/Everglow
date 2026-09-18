using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.Scene;

public class SceneUtils
{
	/// <summary>
	/// Fill draw vertices that cover a tile.(TriangleList)
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="texture"></param>
	/// <param name="startCoordX"></param>
	/// <param name="startCoordY"></param>
	/// <param name="bars"></param>
	public static void DrawTileCover(int i, int j, Texture2D texture, float startCoordX, float startCoordY, List<Vertex2D> bars, bool flipH, float colorFactors)
	{
		Vector2 drawPos0 = new Point(i, j).ToWorldCoordinates() - new Vector2(8);
		Vector2 drawPos1 = drawPos0 + new Vector2(16, 0);
		Vector2 drawPos2 = drawPos0 + new Vector2(0, 16);
		Vector2 drawPos3 = drawPos0 + new Vector2(16, 16);

		Color light0 = Lighting.GetColor(drawPos0.ToTileCoordinates()) * colorFactors;
		Color light1 = Lighting.GetColor(drawPos1.ToTileCoordinates()) * colorFactors;
		Color light2 = Lighting.GetColor(drawPos2.ToTileCoordinates()) * colorFactors;
		Color light3 = Lighting.GetColor(drawPos3.ToTileCoordinates()) * colorFactors;

		Vector2 size = new Vector2(16) / texture.Size();

		float coordX0 = startCoordX;
		float coordX1 = startCoordX + size.X;
		if (flipH)
		{
			(coordX0, coordX1) = (coordX1, coordX0);
		}

		bars.Add(drawPos0, light0, new Vector3(coordX0, startCoordY, 0));
		bars.Add(drawPos1, light1, new Vector3(coordX1, startCoordY, 0));
		bars.Add(drawPos3, light3, new Vector3(coordX1, startCoordY + size.Y, 0));

		bars.Add(drawPos0, light0, new Vector3(coordX0, startCoordY, 0));
		bars.Add(drawPos3, light3, new Vector3(coordX1, startCoordY + size.Y, 0));
		bars.Add(drawPos2, light2, new Vector3(coordX0, startCoordY + size.Y, 0));
	}

	/// <summary>
	/// Make sure the width and the height of the texture are the multiples of 16.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="texture"></param>
	/// <param name="bars"></param>
	public static void DrawMultiSceneTowardBottom(int i, int j, Texture2D texture, List<Vertex2D> bars, bool flipH, float colorFactors = 1)
	{
		for (int x = 0; x < texture.Width; x += 16)
		{
			for (int y = 0; y < texture.Height; y += 16)
			{
				if (flipH)
				{
					DrawTileCover(i - x / 16, j + y / 16, texture, x / (float)texture.Width, y / (float)texture.Height, bars, flipH, colorFactors);
				}
				else
				{
					DrawTileCover(i + x / 16, j + y / 16, texture, x / (float)texture.Width, y / (float)texture.Height, bars, flipH, colorFactors);
				}
			}
		}
	}

	//public static void DrawHangingFlagLike(Vector2 anchorPos, Texture2D texture, List<Vertex2D> bars, bool flipH, float colorFactors = 1)
	//{
	//	Vector2 halfSize = new Vector2(texture.Width * 0.5f, 0);
	//	for (int x = 0; x < texture.Width; x += 16)
	//	{
	//		for (int y = 0; y < texture.Height; y += 16)
	//		{
	//			if (x > texture.Width)
	//			{
	//				x = texture.Width;
	//			}
	//			if (y > texture.Height)
	//			{
	//				y = texture.Height;
	//			}
	//			DrawFlagLikeCover(anchorPos + new Vector2(x, y) - halfSize, texture, x / (float)texture.Width, y / (float)texture.Height, bars, false, colorFactors);
	//		}
	//	}
	//}

	//public static void DrawFlagLikeCover(Vector2 worldPos, Texture2D texture, float startCoordX, float startCoordY, List<Vertex2D> bars, bool flipH, float colorFactors)
	//{
	//	Vector2 drawPos0 = worldPos - new Vector2(8);
	//	Vector2 drawPos1 = drawPos0 + new Vector2(16, 0);
	//	Vector2 drawPos2 = drawPos0 + new Vector2(0, 16);
	//	Vector2 drawPos3 = drawPos0 + new Vector2(16, 16);

	//	Vector2 size = new Vector2(16) / texture.Size();
	//	float weight0 = texture.Size().Y * (1 - startCoordY) / 16f;
	//	float weight1 = texture.Size().Y * (1 - (startCoordY + size.Y)) / 16f;
	//	drawPos0 += GetWindPushBalance(drawPos0, weight0);
	//	drawPos1 += GetWindPushBalance(drawPos1, weight0);
	//	drawPos2 += GetWindPushBalance(drawPos2, weight1);
	//	drawPos3 += GetWindPushBalance(drawPos3, weight1);

	//	Color light0 = Lighting.GetColor(drawPos0.ToTileCoordinates()) * colorFactors;
	//	Color light1 = Lighting.GetColor(drawPos1.ToTileCoordinates()) * colorFactors;
	//	Color light2 = Lighting.GetColor(drawPos2.ToTileCoordinates()) * colorFactors;
	//	Color light3 = Lighting.GetColor(drawPos3.ToTileCoordinates()) * colorFactors;

	//	float coordX0 = startCoordX;
	//	float coordX1 = startCoordX + size.X;
	//	if (flipH)
	//	{
	//		(coordX0, coordX1) = (coordX1, coordX0);
	//	}

	//	bars.Add(drawPos0, light0, new Vector3(coordX0, startCoordY, 0));
	//	bars.Add(drawPos1, light1, new Vector3(coordX1, startCoordY, 0));
	//	bars.Add(drawPos3, light3, new Vector3(coordX1, startCoordY + size.Y, 0));

	//	bars.Add(drawPos0, light0, new Vector3(coordX0, startCoordY, 0));
	//	bars.Add(drawPos3, light3, new Vector3(coordX1, startCoordY + size.Y, 0));
	//	bars.Add(drawPos2, light2, new Vector3(coordX0, startCoordY + size.Y, 0));
	//}
}
