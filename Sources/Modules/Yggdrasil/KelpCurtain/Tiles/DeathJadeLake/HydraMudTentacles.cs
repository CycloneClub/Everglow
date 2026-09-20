using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

[Pipeline(typeof(WCSPipeline))]
public class HydraMudTentacles_fore : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public override void Update()
	{
		if (OriginTilePos.X >= Main.maxTilesX - 1 || OriginTilePos.X <= 1)
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		if (OriginTilePos.Y >= Main.maxTilesY - 1 || OriginTilePos.Y <= 1)
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		if (!VFXManager.InScreen(OriginTilePos.ToWorldCoordinates(), 150))
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		Tile tile = Main.tile[OriginTilePos];
		if (tile.TileType != OriginTileType || !tile.HasTile)
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
	}

	public float scale;
	public int style;

	public override void Draw()
	{
		if (OriginTilePos.X >= Main.maxTilesX - 1 || OriginTilePos.X <= 1)
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		if (OriginTilePos.Y >= Main.maxTilesY - 1 || OriginTilePos.Y <= 1)
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		Tile tile = Main.tile[OriginTilePos];
		if (tile.TileType != OriginTileType || !tile.HasTile)
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		var bars = new List<Vertex2D>();
		if (tile.Slope == SlopeType.Solid && !tile.halfBrick())
		{
			if (tile.TileFrameX >= 18 && tile.TileFrameX <= 54 && tile.TileFrameY == 0)
			{
				AddDrawingFace(bars, new Vector2(0, -1), Position);
			}
			if (tile.TileFrameX >= 18 && tile.TileFrameX <= 54 && tile.TileFrameY == 36)
			{
				AddDrawingFace(bars, new Vector2(0, 1), Position);
			}
			if (tile.TileFrameX == 0 && tile.TileFrameY <= 36 && tile.TileFrameY >= 0)
			{
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
			}
			if (tile.TileFrameX == 72 && tile.TileFrameY <= 36 && tile.TileFrameY >= 0)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
			}
			if (tile.TileFrameX == 90 && tile.TileFrameY <= 36 && tile.TileFrameY >= 0)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
			}
			if (tile.TileFrameY == 72 && tile.TileFrameX >= 108 && tile.TileFrameX <= 144)
			{
				AddDrawingFace(bars, new Vector2(0, 1), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
			}
			if (tile.TileFrameY == 0 && tile.TileFrameX >= 108 && tile.TileFrameX <= 144)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
			}
			if (tile.TileFrameY == 54 && tile.TileFrameX >= 108 && tile.TileFrameX <= 144)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, 1), Position);
			}
			if (tile.TileFrameX == 162 && tile.TileFrameY <= 36 && tile.TileFrameY >= 0)
			{
				AddDrawingFace(bars, new Vector2(0, 1), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
			}
			if (tile.TileFrameX == 216 && tile.TileFrameY <= 36 && tile.TileFrameY >= 0)
			{
				AddDrawingFace(bars, new Vector2(0, 1), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
				AddDrawingFace(bars, new Vector2(1, 0), Position);
			}
			if (tile.TileFrameY == 54 && tile.TileFrameX >= 162 && tile.TileFrameX <= 198)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, 1), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
			}
			if (tile.TileFrameY == 54 && tile.TileFrameX % 36 == 0 && tile.TileFrameX <= 90)
			{
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
			}
			if (tile.TileFrameY == 72 && tile.TileFrameX % 36 == 0 && tile.TileFrameX <= 90)
			{
				AddDrawingFace(bars, new Vector2(-1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, 1), Position);
			}
			if (tile.TileFrameY == 54 && tile.TileFrameX % 36 == 18 && tile.TileFrameX <= 90)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, -1), Position);
			}
			if (tile.TileFrameY == 72 && tile.TileFrameX % 36 == 18 && tile.TileFrameX <= 90)
			{
				AddDrawingFace(bars, new Vector2(1, 0), Position);
				AddDrawingFace(bars, new Vector2(0, 1), Position);
			}
		}
		else
		{
			if (tile.Slope == SlopeType.SlopeUpLeft)
			{
				AddDrawingFace(bars, new Vector2(1, 1) * 0.85f, Position, 1.4143f, new Vector2(-5, -5));
				Tile upTile = Main.tile[OriginTilePos + new Point(0, -1)];
				if (!upTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(0, -1), Position);
				}
				Tile leftTile = Main.tile[OriginTilePos + new Point(-1, 0)];
				if (!leftTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(-1, 0), Position);
				}
				Tile downTile = Main.tile[OriginTilePos + new Point(0, 1)];
				if (downTile.HasTile && downTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(0, -1), Position, 1, new Vector2(0, 16));
				}
			}
			if (tile.Slope == SlopeType.SlopeDownLeft)
			{
				AddDrawingFace(bars, new Vector2(1, -1) * 0.85f, Position, 1.4143f, new Vector2(-5, 5));
				Tile downTile = Main.tile[OriginTilePos + new Point(0, 1)];
				if (!downTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(0, 1), Position);
				}
				Tile leftTile = Main.tile[OriginTilePos + new Point(-1, 0)];
				if (!leftTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(-1, 0), Position);
				}
				Tile upTile = Main.tile[OriginTilePos + new Point(0, -1)];
				if (upTile.HasTile && upTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(0, 1), Position, 1, new Vector2(0, -16));
				}
			}
			if (tile.Slope == SlopeType.SlopeUpRight)
			{
				AddDrawingFace(bars, new Vector2(-1, 1) * 0.85f, Position, 1.4143f, new Vector2(5, -5));
				Tile upTile = Main.tile[OriginTilePos + new Point(0, -1)];
				if (!upTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(0, -1), Position);
				}
				Tile rightTile = Main.tile[OriginTilePos + new Point(1, 0)];
				if (!rightTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(1, 0), Position);
				}
				Tile downTile = Main.tile[OriginTilePos + new Point(0, 1)];
				if (downTile.HasTile && downTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(0, -1), Position, 1, new Vector2(0, 16));
				}
			}
			if (tile.Slope == SlopeType.SlopeDownRight)
			{
				AddDrawingFace(bars, new Vector2(-1, -1) * 0.85f, Position, 1.4143f, new Vector2(5, 5));
				Tile downTile = Main.tile[OriginTilePos + new Point(0, 1)];
				if (!downTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(0, 1), Position);
				}
				Tile rightTile = Main.tile[OriginTilePos + new Point(1, 0)];
				if (!rightTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(1, 0), Position);
				}
				Tile upTile = Main.tile[OriginTilePos + new Point(0, -1)];
				if (upTile.HasTile && upTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(0, 1), Position, 1, new Vector2(0, -16));
				}
			}
			if (tile.halfBrick())
			{
				AddDrawingFace(bars, new Vector2(0, -1), Position, 1, new Vector2(0, 6));
				if (!Main.tile[OriginTilePos + new Point(0, 1)].HasTile)
				{
					AddDrawingFace(bars, new Vector2(0, 1), Position);
				}
				Tile upTile = Main.tile[OriginTilePos + new Point(0, -1)];
				if (upTile.HasTile && upTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(0, 1), Position, 1, new Vector2(0, -16));
				}
				Tile leftTile = Main.tile[OriginTilePos + new Point(-1, 0)];
				if (leftTile.HasTile && leftTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(1, 0), Position, 0.5f, new Vector2(-14, -4));
				}
				if (!leftTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(-1, 0), Position, 0.5f, new Vector2(0, 4));
				}
				Tile rightTile = Main.tile[OriginTilePos + new Point(1, 0)];
				if (rightTile.HasTile && rightTile.TileType == OriginTileType)
				{
					AddDrawingFace(bars, new Vector2(-1, 0), Position, 0.5f, new Vector2(14, -4));
				}
				if (!rightTile.HasTile)
				{
					AddDrawingFace(bars, new Vector2(1, 0), Position, 0.5f, new Vector2(0, 4));
				}
			}
		}
		Ins.Batch.Draw(ModAsset.HydraMudTentacles.Value, bars, PrimitiveType.TriangleList);
	}

	public void AddDrawingFace(List<Vertex2D> bars, Vector2 Direction, Vector2 Position, float length = 1, Vector2 offset = default)
	{
		Tile tile = Main.tile[OriginTilePos];
		if (tile.Slope == SlopeType.Solid && !tile.halfBrick())
		{
			Tile directionTile = Main.tile[OriginTilePos + Direction.ToPoint()];
			if (TileUtils.IsTileSolid(directionTile))
			{
				return;
			}
		}

		Vector2 drawCenter = Position + new Vector2(8) + offset;
		var directionN = Vector2.Normalize(Direction);
		float texWidth = 64f;
		float frameWidth = 16f;
		float rot = -MathF.Asin(Vector3.Cross(new Vector3(directionN, 0), new Vector3(0, -1, 0)).Z);
		float cos = Vector2.Dot(directionN, new Vector2(0, -1));
		if (cos < 0)
		{
			rot = MathHelper.Pi - rot;
		}
		float frameYModify = 0f;
		if (cos < 0.2f)
		{
			frameYModify = 0.5f;
		}
		float dLength = 16 / texWidth;
		float sway0 = MathF.Sin((float)Main.time / 36f + Position.X + Position.Y) * 3;
		float sway1 = MathF.Sin((float)Main.time / 36f + Position.X + Position.Y + 32) * 3;
		Vector2 point0 = drawCenter + new Vector2(-8 * length + sway0, -18).RotatedBy(rot) + Direction * 8;
		Vector2 point1 = drawCenter + new Vector2(8 * length + sway1, -18).RotatedBy(rot) + Direction * 8;
		Vector2 point2 = drawCenter + new Vector2(-8 * length, 6).RotatedBy(rot) + Direction * 12;
		Vector2 point3 = drawCenter + new Vector2(8 * length, 6).RotatedBy(rot) + Direction * 12;

		Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		bars.Add(point0, color, new Vector3(frameWidth * style / texWidth, frameYModify, 0));
		bars.Add(point1, color, new Vector3(dLength + frameWidth * style / texWidth, frameYModify, 0));
		bars.Add(point3, color, new Vector3(dLength + frameWidth * style / texWidth, 0.5f + frameYModify, 0));

		bars.Add(point2, color, new Vector3(frameWidth * style / texWidth, 0.5f + frameYModify, 0));
		bars.Add(point0, color, new Vector3(frameWidth * style / texWidth, frameYModify, 0));
		bars.Add(point3, color, new Vector3(dLength + frameWidth * style / texWidth, 0.5f + frameYModify, 0));
	}
}
