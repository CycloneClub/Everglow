using System.Runtime.InteropServices;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.Utilities;

public static class SpriteBatchUtils
{
	public static List<Vertex2D> DrawCurveStrip_EnvironmentLight(List<Vector2> curve, float width, float coord_x_min, float coord_x_max, float coord_y_min = 0, float coord_y_max = 1, bool curveHasScreenPos = false)
	{
		if (curve.Count < 2)
		{
			return [];
		}
		Vector2 lightSamplingOffset = Vector2.zeroVector;
		if (!curveHasScreenPos)
		{
			lightSamplingOffset = Main.screenPosition;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < curve.Count; i++)
		{
			Vector2 pos = curve[i];
			Vector2 dir;
			if (i == 0)
			{
				dir = curve[i + 1] - curve[i];
			}
			else
			{
				dir = curve[i] - curve[i - 1];
			}
			dir = dir.NormalizeSafe();
			Vector2 normal = new Vector2(dir.Y, -dir.X) * width / 2f;
			float value = i / (float)(curve.Count - 1);
			float coordX = float.Lerp(coord_x_min, coord_x_max, value);
			AddVertexWithEnv_Light(bars, pos + normal + lightSamplingOffset, new Vector3(coordX, coord_y_min, 0));
			AddVertexWithEnv_Light(bars, pos - normal + lightSamplingOffset, new Vector3(coordX, coord_y_max, 0));
		}
		return bars;
	}

	public static Color GetEnv_Light(Vector2 worldPosition)
	{
		return Lighting.GetColor(worldPosition.ToTileCoordinates());
	}

	public static void AddVertexWithEnv_Light(List<Vertex2D> bars, Vector2 worldPos, Vector3 texCoord, bool removeScreenPos = true, float colorFade = 1f)
	{
		Vector2 offset = Vector2.Zero;
		if (removeScreenPos)
		{
			offset = -Main.screenPosition;
		}
		bars.Add(worldPos + offset, GetEnv_Light(worldPos + Main.screenPosition + offset) * colorFade, texCoord);
	}

	public static void AddVerticesForCircleRing(List<Vertex2D> bars, Vector2 position, float radius, float width, Color drawColor, float coorx_x_min, float coord_x_max, float coord_z = 0)
	{
		int currentSize = bars.Count;
		int count = (int)(radius / 4 + 10);
		CollectionsMarshal.SetCount(bars, count * 2 + 256);
		Span<Vertex2D> span = CollectionsMarshal.AsSpan(bars);
		for (int k = 0; k <= count; k++)
		{
			float value = k / (float)count;
			float coordX = (float)Utils.Lerp(coorx_x_min, coord_x_max, value);
			Vector2 pos0 = position + new Vector2(0, radius + width / 2f).RotatedBy(value * MathHelper.TwoPi);
			Vector2 pos1 = position + new Vector2(0, radius - width / 2f).RotatedBy(value * MathHelper.TwoPi);
			float maxCoordY = 1;
			if (radius < width / 2f)
			{
				pos1 = position;
				maxCoordY = 0.5f + radius / (width / 2f);
			}
			span[currentSize] = new Vertex2D(pos0, drawColor, new Vector3(coordX, 0, coord_z));
			currentSize++;
			span[currentSize] = new Vertex2D(pos1, drawColor, new Vector3(coordX, maxCoordY, coord_z));
			currentSize++;
		}
		CollectionsMarshal.SetCount(bars, currentSize);
	}

	/// <summary>
	/// R:0-1<br/>G:0-1<br/>B:0-1
	/// </summary>
	/// <param name="rgb">R:0-1<br/>G:0-1<br/>B:0-1</param>
	/// <returns>H:0-360<br/>S:0-1<br/>V:0-1</returns>
	public static Vector3 RGBToHSV(Vector3 rgb)
	{
		float r = rgb.X;
		float g = rgb.Y;
		float b = rgb.Z;

		float max = MathHelper.Max(MathHelper.Max(r, g), b);
		float min = MathHelper.Min(MathHelper.Min(r, g), b);
		float delta = max - min;

		float h = 0f;
		float s = max == 0 ? 0f : delta / max;
		float v = max;

		if (delta == 0)
		{
			h = 0f;
		}
		else
		{
			if (max == r)
			{
				h = ((g - b) / delta) % 6;
			}
			else if (max == g)
			{
				h = 2 + (b - r) / delta;
			}
			else if (max == b)
			{
				h = 4 + (r - g) / delta;
			}

			h *= 60;
			if (h < 0)
			{
				h += 360;
			}
		}

		return new Vector3(h, s, v);
	}

	/// <summary>
	/// H:0-360<br/>S:0-1<br/>V:0-1
	/// </summary>
	/// <param name="hsv">H:0-360<br/>S:0-1<br/>V:0-1</param>
	/// <returns>R:0-1<br/>G:0-1<br/>B:0-1</returns>
	public static Vector3 HSVToRGB(Vector3 hsv)
	{
		float h = hsv.X;
		float s = hsv.Y;
		float v = hsv.Z;

		if (s == 0)
		{
			return new Vector3(v, v, v);
		}

		h = h % 360f;
		if (h < 0)
		{
			h += 360;
		}

		float sector = h / 60f;
		int sectorIndex = (int)Math.Floor(sector);
		float fractional = sector - sectorIndex;

		float p = v * (1 - s);
		float q = v * (1 - s * fractional);
		float t = v * (1 - s * (1 - fractional));

		switch (sectorIndex)
		{
			case 0:
				return new Vector3(v, t, p);
			case 1:
				return new Vector3(q, v, p);
			case 2:
				return new Vector3(p, v, t);
			case 3:
				return new Vector3(p, q, v);
			case 4:
				return new Vector3(t, p, v);
			default:
				return new Vector3(v, p, q);
		}
	}

	/// <summary>
	/// Transforms Vector3(H, S, V) to RGB and returns a Color with the specified alpha.
	/// </summary>
	/// <param name="hsv">H:0-360<br/>S:0-1<br/>V:0-1</param>
	/// <param name="a">0-1</param>
	/// <returns></returns>
	public static Color HSVToRGB_Color(this Vector3 hsv, float a)
	{
		Vector3 rgb = HSVToRGB(hsv);
		return new Color(rgb.X, rgb.Y, rgb.Z, a);
	}

	/// <summary>
	/// Add vertices just like what Main.spriteBatch.Draw does. But sampling environment light at each vertex.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="frame"></param>
	/// <param name="origin"></param>
	/// <param name="tex"></param>
	/// <param name="screenPos"></param>
	/// <param name="rotation"></param>
	/// <param name="alpha"></param>
	public static void AddVertex_Grid(List<Vertex2D> bars, Vector2 position, Rectangle? frame, Vector2 origin, Texture2D tex, bool screenPos = false, int gridSize = 16, float rotation = 0, float alpha = 1f)
	{
		var drawPos = position;
		Vector2 pos = drawPos;
		if (screenPos)
		{
			pos += Main.screenPosition;
		}
		Rectangle frame2 = frame ?? tex.Frame();
		int xCount = frame2.Width / gridSize;
		int yCount = frame2.Height / gridSize;
		float unitX = frame2.Width / (float)xCount;
		float unitY = frame2.Height / (float)yCount;
		for (int x = 0; x < xCount; x++)
		{
			Vector2 offset_2 = (new Vector2(x * unitX, 0) - origin).RotatedBy(rotation);
			Vector2 offset_1 = (new Vector2((x + 1) * unitX, 0) - origin).RotatedBy(rotation);
			bars.Add(pos + offset_2, Color.Transparent, new Vector3(new Vector2(frame2.X + x * unitX, frame2.Y) / tex.Size(), 0));
			bars.Add(pos + offset_1, Color.Transparent, new Vector3(new Vector2(frame2.X + (x + 1) * unitX, frame2.Y) / tex.Size(), 0));
			for (int y = 0; y < yCount; y++)
			{
				Vector2 offset0 = (new Vector2(x * unitX, y * unitY) - origin).RotatedBy(rotation);
				Vector2 offset1 = (new Vector2((x + 1) * unitX, y * unitY) - origin).RotatedBy(rotation);
				Vector2 offset2 = (new Vector2(x * unitX, (y + 1) * unitY) - origin).RotatedBy(rotation);
				Vector2 offset3 = (new Vector2((x + 1) * unitX, (y + 1) * unitY) - origin).RotatedBy(rotation);

				AddLightColorVertex(bars, pos + offset0, new Vector3(new Vector2(frame2.X + x * unitX, frame2.Y + y * unitY) / tex.Size(), 0), alpha, screenPos);
				AddLightColorVertex(bars, pos + offset1, new Vector3(new Vector2(frame2.X + (x + 1) * unitX, frame2.Y + y * unitY) / tex.Size(), 0), alpha, screenPos);
				AddLightColorVertex(bars, pos + offset2, new Vector3(new Vector2(frame2.X + x * unitX, frame2.Y + (y + 1) * unitY) / tex.Size(), 0), alpha, screenPos);
				AddLightColorVertex(bars, pos + offset3, new Vector3(new Vector2(frame2.X + (x + 1) * unitX, frame2.Y + (y + 1) * unitY) / tex.Size(), 0), alpha, screenPos);
			}
			Vector2 offset4 = (new Vector2(x * unitX, yCount * unitY) - origin).RotatedBy(rotation);
			Vector2 offset5 = (new Vector2((x + 1) * unitX, yCount * unitY) - origin).RotatedBy(rotation);
			bars.Add(pos + offset4, Color.Transparent, new Vector3(new Vector2(frame2.X + x * unitX, frame2.Y + yCount * unitY) / tex.Size(), 0));
			bars.Add(pos + offset5, Color.Transparent, new Vector3(new Vector2(frame2.X + (x + 1) * unitX, frame2.Y + yCount * unitY) / tex.Size(), 0));
		}
	}

	public static void AddLightColorVertex(List<Vertex2D> bars, Vector2 worldPos, Vector3 coord, float alpha = 1f, bool screenPos = false)
	{
		Color drawC = Lighting.GetColor(worldPos.ToTileCoordinates()) * alpha;
		if (screenPos)
		{
			worldPos -= Main.screenPosition;
		}
		bars.Add(worldPos, drawC, coord);
	}
}
