using Everglow.Commons.Physics.Abstracts;
using Everglow.Commons.Physics.Colliders;

namespace Everglow.Commons.Utilities;

public class SDFUtils
{
	[Obsolete("Not all tiles are supported.", true)]
	public static Vector3 CalculateTileSDF(Vector2 pos)
	{
		int tileX = (int)Math.Floor(pos.X / 16);
		int tileY = (int)Math.Floor(pos.Y / 16);

		if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
		{
			return new Vector3(16, 0, 0);
		}

		bool inner = false;
		if (Main.tile[tileX, tileY].HasTile)
		{
			var tile = Main.tile[tileX, tileY];
			var collider = SDFUtils.ExtractColliderFromTile(tile, tileX, tileY);
			if (collider != null && collider.Contains(pos))
			{
				inner = true;
			}
		}

		Vector3 targetSDF = new Vector3(16, 0, 0);
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (tileX + j < 0 || tileX + j >= Main.maxTilesX || tileY + i < 0 || tileY + i >= Main.maxTilesY)
				{
					continue;
				}
				var tile = Main.tile[tileX + j, tileY + i];
				var collider = SDFUtils.ExtractColliderFromTile(tile, tileX + j, tileY + i, inner);

				if (collider != null)
				{
					var sdf = collider.GetSDFWithGradient(pos);
					if (sdf.X < targetSDF.X)
					{
						targetSDF = sdf;
					}
					//// Is solid block
					// if (tile.HasTile && Main.tileSolid[tile.TileType])
					// {
					//    if (sdf.X < solidSDF.X)
					//    {
					//        solidSDF = sdf;
					//    }
					// }
					// else
					// {
					//    if (sdf.X < airSDF.X)
					//    {
					//        airSDF = sdf;
					//    }
					// }
				}
			}
		}

		if (inner)
		{
			return new Vector3(-targetSDF.X, targetSDF.Y, targetSDF.Z);
		}
		else
		{
			return targetSDF;
		}
	}

	[Obsolete("Not all tiles are supported.", true)]
	public static ICollider2D ExtractColliderFromTile(in Tile tile, int i, int j, bool inverse = false)
	{
		if (!tile.HasTile || !Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])
		{
			if (inverse)
			{
				return new AABBCollider2D()
				{
					Center = new Vector2(i * 16 + 8, j * 16 + 8),
					Size = new Vector2(8, 8),
				};
			}
			return null;
		}

		if (tile.BlockType is BlockType.Solid or BlockType.HalfBlock)
		{
			if (tile.IsHalfBlock)
			{
				return inverse ? new AABBCollider2D()
				{
					Center = new Vector2(i * 16 + 8, j * 16 + 4),
					Size = new Vector2(8, 4),
				}
				: new AABBCollider2D()
				{
					Center = new Vector2(i * 16 + 8, j * 16 + 12),
					Size = new Vector2(8, 4),
				};
			}
			else
			{
				return inverse ? null : new AABBCollider2D()
				{
					Center = new Vector2(i * 16 + 8, j * 16 + 8),
					Size = new Vector2(8, 8),
				};
			}
		}
		else
		{
			Vector2 dirLine = Vector2.One;
			if (tile.Slope == SlopeType.SlopeUpRight)
			{
				dirLine = -dirLine;
			}
			else if (tile.Slope == SlopeType.SlopeDownRight)
			{
				dirLine = new Vector2(1, -1);
			}
			else if (tile.Slope == SlopeType.SlopeUpLeft)
			{
				dirLine = new Vector2(-1, 1);
			}

			// 斜坡只要线方向相反就是反形状
			return new TriangleCollider2D()
			{
				Center = new Vector2(i * 16 + 8, j * 16 + 8),
				Size = new Vector2(8, 8),
				LineDir = inverse ? -dirLine : dirLine,
			};
		}
	}

	public static Vector3 SdgBox(Vector2 p, Vector2 b)
	{
		Vector2 w = new Vector2(Math.Abs(p.X), Math.Abs(p.Y)) - b;
		var s = new Vector2(p.X < 0.0 ? -1 : 1, p.Y < 0.0 ? -1 : 1);
		float g = Math.Max(w.X, w.Y);
		var q = new Vector2(Math.Max(w.X, 0.0f), Math.Max(w.Y, 0.0f));
		float l = q.Length();
		var v = s * (g > 0.0 ? q / l : w.X > w.Y ? new Vector2(1, 0) : new Vector2(0, 1));
		return new Vector3(g > 0.0 ? l : g, v.X, v.Y);
	}

	private Vector3 SdgSmoothMin(Vector3 a, Vector3 b, float k)
	{
		float h = Math.Max(k - Math.Abs(a.X - b.X), 0.0f);
		float m = 0.25f * h * h / k;
		float n = 0.50f * h / k;
		var v = Vector2.Lerp(new Vector2(a.Y, a.Z), new Vector2(b.Y, b.Z), a.X < b.X ? n : 1.0f - n);
		return new Vector3(Math.Min(a.X, b.X) - m, v.X, v.Y);
	}

	public static Vector3 SdgLine(Vector2 p, Vector2 d)
	{
		// 直线的SDF
		d.Normalize();
		Vector2 proj = Vector2.Dot(d, p) * d;
		Vector2 N = p - proj;
		float x = Math.Sign(-MathUtils.Cross(d, p)) * N.Length();
		if (N != Vector2.zeroVector)
		{
			N = N.NormalizeSafe();
		}
		else
		{
			N = new Vector2(-d.Y, d.X);
		}
		return new Vector3(x, N.X, N.Y);
	}

	// private Vector3 sdgSegment(in Vector2 p, in Vector2 a, in Vector2 b)
	// {
	//    // 直线的SDF
	//    Vector2 proj = Vector2.Dot(d, p) / d.Length() * d;
	//    Vector2 N = p - proj;
	//    float x = Math.Sign(cross(d, p)) * N.Length();
	//    return new Vector3(x, N.X, N.Y);
	// }
}
