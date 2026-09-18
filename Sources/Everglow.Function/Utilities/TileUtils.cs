using Everglow.Commons.Vertex;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Commons.Utilities;

public partial class TileUtils
{
	public static bool IsType<T>(this Tile tile)
		where T : ModTile
	{
		var modTile = TileLoader.GetTile(tile.type);
		return modTile is not null && modTile is T;
	}

	public static int ToTileCoordinate(this float value) => (int)value >> 4;

	public static int ToTileCoordinate(this double value) => (int)value >> 4;

	public static Tile SafeGetTile(int i, int j) =>
		Main.tile[Math.Clamp(i, 20, Main.maxTilesX - 20), Math.Clamp(j, 20, Main.maxTilesY - 20)];

	public static Tile SafeGetTile(Point point) => SafeGetTile(point.X, point.Y);

	/// <summary>
	/// The vector is expected to be the tile coordinate, not world coordinate. (i.e. vector.X is expected to be i, and vector.Y is expected to be j)
	/// </summary>
	/// <param name="vector"></param>
	/// <returns></returns>
	public static Tile SafeGetTile(Vector2 vector) => SafeGetTile((int)vector.X, (int)vector.Y);

	public static void DefaultToMultiTileAnchorBottom(int width, int height)
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = height;
		TileObjectData.newTile.Width = width;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateHeights = new int[height];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.CoordinateHeights[^1] = 18;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.Origin = new Point16(width / 2, height - 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, width, 0);
	}

	public static void DefaultToMultiTileAnchorTop(int width, int height)
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = height;
		TileObjectData.newTile.Width = width;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateHeights = new int[height];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.CoordinateHeights[^1] = 18;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.Origin = new Point16(width / 2, height - 1);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, width, 0);
	}

	public static void DefaultToMultiTileWall(int width, int height)
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Height = height;
		TileObjectData.newTile.Width = width;
		TileObjectData.newTile.CoordinateHeights = new int[height];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.Origin = new Point16(width / 2, height / 2);
		TileObjectData.newTile.StyleHorizontal = true;
	}

	public static int GetFixedRandomNumber(int x, int y, int max = 1024)
	{
		Random random = new Random(SafeGetTile(Math.Abs(x), Math.Abs(y)).GetHashCode());
		return random.Next(0, max);
	}

	public static int GetFixedRandomNumber(Point point, int max = 1024)
	{
		Random random = new Random(SafeGetTile(point).GetHashCode());
		return random.Next(0, max);
	}

	public static int GetFixedRandomNumber(Tile tile, int max = 1024)
	{
		Random random = new Random(tile.GetHashCode());
		return random.Next(0, max);
	}

	public static int GetFixedRandomNumber_SingleSeed(int seed, int max = 1024)
	{
		Random random = new Random(seed);
		return random.Next(0, max);
	}

	public static bool AreaHasTile(int x, int y, int width, int height, Func<Tile, bool> prediction = null)
	{
		for (int i = x; i < x + width; i++)
		{
			for (int j = y; j < y + height; j++)
			{
				var tile = SafeGetTile(i, j);
				if (tile.HasTile && (prediction is null || prediction(tile)))
				{
					return true;
				}
			}
		}

		return false;
	}

	public static int X(this Tile tile)
	{
		return (tile.GetHashCode() - tile.Y()) / Main.tile.Height;
	}

	public static int Y(this Tile tile)
	{
		return tile.GetHashCode() % Main.tile.Height;
	}

	public static Vector2 Center(this Tile tile)
	{
		return new Point(tile.X(), tile.Y()).ToWorldCoordinates();
	}

	public static void VertexDraw_4_Corner(Vector2 drawCenterPos, Rectangle frame, Vector2 origin, Texture2D tex, SpriteBatch spriteBatch, float rotation = 0)
	{
		var drawPos = drawCenterPos;
		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2 pos = drawPos + Main.screenPosition;
		Vector2 offset0 = (new Vector2(0, 0) - origin).RotatedBy(rotation);
		Vector2 offset1 = (new Vector2(frame.Width, 0) - origin).RotatedBy(rotation);
		Vector2 offset2 = (new Vector2(0, frame.Height) - origin).RotatedBy(rotation);
		Vector2 offset3 = (new Vector2(frame.Width, frame.Height) - origin).RotatedBy(rotation);

		AddLightColorVertex(bars, pos + offset0, new Vector3(new Vector2(frame.X, frame.Y) / tex.Size(), 0));
		AddLightColorVertex(bars, pos + offset1, new Vector3(new Vector2(frame.X + frame.Width, frame.Y) / tex.Size(), 0));
		AddLightColorVertex(bars, pos + offset2, new Vector3(new Vector2(frame.X, frame.Y + frame.Height) / tex.Size(), 0));
		AddLightColorVertex(bars, pos + offset3, new Vector3(new Vector2(frame.X + frame.Width, frame.Y + frame.Height) / tex.Size(), 0));
		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public static void VertexDraw_Grid(Vector2 drawCenterPos, Rectangle frame, Vector2 origin, Texture2D tex, SpriteBatch spriteBatch, float rotation = 0, float alpha = 1f)
	{
		var drawPos = drawCenterPos;
		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2 pos = drawPos + Main.screenPosition;
		int xCount = frame.Width / 16;
		int yCount = frame.Height / 16;
		float unitX = frame.Width / (float)xCount;
		float unitY = frame.Height / (float)yCount;
		for (int x = 0; x < xCount; x++)
		{
			for (int y = 0; y < yCount; y++)
			{
				Vector2 offset0 = (new Vector2(x * unitX, y * unitY) - origin).RotatedBy(rotation);
				Vector2 offset1 = (new Vector2((x + 1) * unitX, y * unitY) - origin).RotatedBy(rotation);
				Vector2 offset2 = (new Vector2(x * unitX, (y + 1) * unitY) - origin).RotatedBy(rotation);
				Vector2 offset3 = (new Vector2((x + 1) * unitX, (y + 1) * unitY) - origin).RotatedBy(rotation);

				AddLightColorVertex(bars, pos + offset0, new Vector3(new Vector2(frame.X + x * unitX, frame.Y + y * unitY) / tex.Size(), 0), alpha);
				AddLightColorVertex(bars, pos + offset1, new Vector3(new Vector2(frame.X + (x + 1) * unitX, frame.Y + y * unitY) / tex.Size(), 0), alpha);
				AddLightColorVertex(bars, pos + offset2, new Vector3(new Vector2(frame.X + x * unitX, frame.Y + (y + 1) * unitY) / tex.Size(), 0), alpha);
				AddLightColorVertex(bars, pos + offset3, new Vector3(new Vector2(frame.X + (x + 1) * unitX, frame.Y + (y + 1) * unitY) / tex.Size(), 0), alpha);
			}
		}

		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public static void AddLightColorVertex(List<Vertex2D> bars, Vector2 worldPos, Vector3 coord, float alpha = 1f)
	{
		Color drawC = Lighting.GetColor(worldPos.ToTileCoordinates()) * alpha;
		bars.Add(worldPos - Main.screenPosition, drawC, coord);
	}
}
