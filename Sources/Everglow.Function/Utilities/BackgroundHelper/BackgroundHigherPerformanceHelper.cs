using System.Runtime.InteropServices;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.Commons.Utilities.BackgroundHelper;

/// <summary>
/// Use unsafe + Lambda closure for extremely high performance rendering.
/// </summary>
public static class BackgroundHigherPerformanceHelper
{
	// Precompute rotation offset (global static, initialized only once)
	private static readonly Vector2[] _rotOffsets =
	[
		new Vector2(0, -8),
		new Vector2(0, 8),
	];

	/// <summary>
	/// Row-major TriangleStrip rendering, closure capture invariants, unsafe pointer vertex writes.<br/>
	/// </summary>
	public static void Add_TileBgVertice(
		BackgroundSlideBase bg,
		List<Point> tiles,
		List<Vertex2D> bars,
		int samplingUnit = 1)
	{
		if (bg.UseColorStyle != 1)
		{
			Add_TileBgVertice_NolightMap(bg, tiles, bars);
			return;
		}

		Vector2 screenHalf = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		Vector2 screenCenter = Main.screenPosition + screenHalf;
		Vector2 texSize = bg.Texture.Size();
		Vector2 texMoveBase = (screenCenter - bg.WorldAnchor) / bg.Distance / texSize;
		Vector2 screenPos = Main.screenPosition;

		// Expand the List in advance to avoid array overflow during unsafe writing (estimate the number of vertices, can be enlarged as needed)
		int currentSize = bars.Count;
		int estimatedCapacity = bars.Count + tiles.Count * 6 + 256;
		if (bars.Capacity < estimatedCapacity)
		{
			bars.Capacity = estimatedCapacity;
		}
		CollectionsMarshal.SetCount(bars, estimatedCapacity);
		Span<Vertex2D> span = CollectionsMarshal.AsSpan(bars);

		var visibleTiles = tiles
		   .Select(p => new { Pos = p, World = p.ToWorldCoordinates() })
		   .Where(t => VFXManager.InScreen(t.World, 64))
		   .OrderBy(t => t.Pos.Y)
		   .ThenBy(t => t.Pos.X)
		   .ToList();

		if (visibleTiles.Count == 0)
		{
			CollectionsMarshal.SetCount(bars, currentSize);
			return;
		}

		int lastY = visibleTiles[0].Pos.Y;
		int lastX = visibleTiles[0].Pos.X;

		if (bg is UnderBackgroundBackgroundSlideBase)
		{
			UnderBackgroundBackgroundSlideBase ubg = bg as UnderBackgroundBackgroundSlideBase;
			if (ubg != null)
			{
				samplingUnit = ubg.SamplingUnit;
			}
		}

		foreach (var tile in visibleTiles)
		{
			Vector2 basePos = tile.World;
			if (tile.Pos.Y != lastY || tile.Pos.X - lastX > samplingUnit)
			{
				// Degenerated vertex: Reuse the last vertex and transparently connect the up and down directions
				// Triangle strip, just like:
				// ↓↗↓↗↓↗↓↗↓↗↓↑↓
				// ↑↓↗↓↗↓↗↓↗↓↗↓↗↓
				// or
				// ↓↗↓↗↓↗↓↗↓↗↓↑↓     ↑↓↗↓↗↓↗↓↗↓↗↓↗↓
				Vertex2D degenerate = span[currentSize - 1];
				degenerate.color = Color.Transparent;

				span[currentSize] = degenerate;
				span[currentSize].position += new Vector2(0, -16 * samplingUnit);
				currentSize++;

				span[currentSize] = degenerate;
				currentSize++;

				Vector2 finalPos = basePos + new Vector2(8, 8) * samplingUnit - Main.screenPosition;
				span[currentSize] = degenerate;
				span[currentSize].position = finalPos + new Vector2(0, -8 * samplingUnit);
				currentSize++;

				span[currentSize] = degenerate;
				span[currentSize].position = finalPos + new Vector2(0, 8 * samplingUnit);
				currentSize++;

				lastY = tile.Pos.Y;
			}
			lastX = tile.Pos.X;

			// Add vertex.
			foreach (var offset in _rotOffsets)
			{
				Vector2 pos = basePos + offset * samplingUnit + new Vector2(8, 8) * samplingUnit;

				Vector2 screenOffset = pos - screenCenter;
				Vector2 uv = screenOffset / texSize + texMoveBase;
				uv.X += 0.5f;
				uv.Y += 0.5f;

				Color drawColor = GetColor(bg, pos);
				Vector3 uv3 = new Vector3(uv.X, uv.Y, 0f);
				Vertex2D vtx = new Vertex2D(pos - screenPos, drawColor, uv3);

				span[currentSize] = vtx;
				currentSize++;
			}
		}

		// Update size.
		CollectionsMarshal.SetCount(bars, currentSize);
	}

	public static void Add_TileBgVertice_NolightMap(
	BackgroundSlideBase bg,
	List<Point> tiles,
	List<Vertex2D> bars)
	{
		Vector2 screenHalf = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		Vector2 screenCenter = Main.screenPosition + screenHalf;
		Vector2 texSize = bg.Texture.Size();
		Vector2 texMoveBase = (screenCenter - bg.WorldAnchor) / bg.Distance / texSize;
		Vector2 screenPos = Main.screenPosition;
		Color drawColor = GetColor(bg, screenCenter);

		// Expand the List in advance to avoid array overflow during unsafe writing (estimate the number of vertices, can be enlarged as needed)
		int currentSize = bars.Count;
		int estimatedCapacity = bars.Count + tiles.Count * 6 + 256;
		if (bars.Capacity < estimatedCapacity)
		{
			bars.Capacity = estimatedCapacity;
		}
		CollectionsMarshal.SetCount(bars, estimatedCapacity);
		Span<Vertex2D> span = CollectionsMarshal.AsSpan(bars);

		var visibleTiles = tiles
		   .Select(p => new { Pos = p, World = p.ToWorldCoordinates() })
		   .Where(t => VFXManager.InScreen(t.World, 64))
		   .OrderBy(t => t.Pos.Y)
		   .ThenBy(t => t.Pos.X)
		   .ToList();

		if (visibleTiles.Count == 0)
		{
			CollectionsMarshal.SetCount(bars, currentSize);
			return;
		}

		int lastY = visibleTiles[0].Pos.Y;
		int lastX = visibleTiles[0].Pos.X;

		int samplingUnit = 1;
		if (bg is UnderBackgroundBackgroundSlideBase)
		{
			UnderBackgroundBackgroundSlideBase ubg = bg as UnderBackgroundBackgroundSlideBase;
			if (ubg != null)
			{
				samplingUnit = ubg.SamplingUnit;
			}
		}

		// Add vertex
		foreach (var offset in _rotOffsets)
		{
			Vector2 pos = visibleTiles[0].World + offset * samplingUnit + new Vector2(8, 8) * samplingUnit;

			Vector2 screenOffset = pos - screenCenter;
			Vector2 uv = screenOffset / texSize + texMoveBase;
			uv.X += 0.5f;
			uv.Y += 0.5f;

			Vector3 uv3 = new Vector3(uv.X, uv.Y, 0f);
			Vertex2D vtx = new Vertex2D(pos - screenPos, drawColor, uv3);

			span[currentSize] = vtx;
			currentSize++;
		}
		foreach (var tile in visibleTiles)
		{
			Vector2 basePos = tile.World;
			if (tile.Pos.Y != lastY || tile.Pos.X - lastX > samplingUnit)
			{
				// Add vertex.
				foreach (var offset in _rotOffsets)
				{
					Vector2 pos = new Point(lastX, lastY).ToWorldCoordinates() + offset * samplingUnit + new Vector2(8, 8) * samplingUnit;

					Vector2 screenOffset = pos - screenCenter;
					Vector2 uv = screenOffset / texSize + texMoveBase;
					uv.X += 0.5f;
					uv.Y += 0.5f;

					Vector3 uv3 = new Vector3(uv.X, uv.Y, 0f);
					Vertex2D vtx = new Vertex2D(pos - screenPos, drawColor, uv3);

					span[currentSize] = vtx;
					currentSize++;
				}

				// Degenerated vertex: Reuse the last vertex and transparently connect the up and down directions
				// Triangle strip, just like:
				// ↓________↗↓↑↓
				Vertex2D degenerate = span[currentSize - 1];
				degenerate.color = Color.Transparent;

				span[currentSize] = degenerate;
				span[currentSize].position += new Vector2(0, -16) * samplingUnit;
				currentSize++;

				span[currentSize] = degenerate;
				currentSize++;

				Vector2 finalPos = basePos + new Vector2(8, 8) * samplingUnit - Main.screenPosition;
				span[currentSize] = degenerate;
				span[currentSize].position = finalPos + new Vector2(0, -8) * samplingUnit;
				currentSize++;

				span[currentSize] = degenerate;
				span[currentSize].position = finalPos + new Vector2(0, 8) * samplingUnit;
				currentSize++;

				lastY = tile.Pos.Y;

				// Add vertex.
				foreach (var offset in _rotOffsets)
				{
					Vector2 pos = basePos + offset * samplingUnit + new Vector2(8, 8) * samplingUnit;

					Vector2 screenOffset = pos - screenCenter;
					Vector2 uv = screenOffset / texSize + texMoveBase;
					uv.X += 0.5f;
					uv.Y += 0.5f;

					Vector3 uv3 = new Vector3(uv.X, uv.Y, 0f);
					Vertex2D vtx = new Vertex2D(pos - screenPos, drawColor, uv3);

					span[currentSize] = vtx;
					currentSize++;
				}
			}
			lastX = tile.Pos.X;
		}

		// Update size.
		CollectionsMarshal.SetCount(bars, currentSize);
	}

	public static void Add_WorldPosVertex(BackgroundSlideBase bg, Vector2 position, Span<Vertex2D> span, ref int currentSize)
	{
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		Vector2 move = (screenCenter - bg.WorldAnchor) / bg.Distance;
		Vector2 texcoord_Move = move / bg.Texture.Size();
		Vector2 screenPoint = position - screenCenter;
		Vector2 screen_move = screenPoint / bg.Texture.Size() + texcoord_Move;

		Color drawColor = GetColor(bg, position);
		span[currentSize] = new Vertex2D(position - Main.screenPosition, drawColor, new Vector3(screen_move + new Vector2(0.5f), 0));
		currentSize++;
	}

	public static void Add_WorldTriangle(BackgroundSlideBase bg, Vector2 v0, Vector2 v1, Vector2 v2, Span<Vertex2D> span, ref int currentSize)
	{
		Add_WorldPosVertex(bg, v0, span, ref currentSize);
		Add_WorldPosVertex(bg, v1, span, ref currentSize);
		Add_WorldPosVertex(bg, v2, span, ref currentSize);
	}

	private static Color GetColor(BackgroundSlideBase bg, Vector2 worldPos)
	{
		Color c = Color.White;
		switch (bg.UseColorStyle)
		{
			case 0:
				c = Main.ColorOfTheSkies;
				break;
			case 1:
				c = Lighting.GetColor((worldPos + new Vector2(-8)).ToTileCoordinates());
				break;
		}
		return c * bg.Alpha;
	}
}
