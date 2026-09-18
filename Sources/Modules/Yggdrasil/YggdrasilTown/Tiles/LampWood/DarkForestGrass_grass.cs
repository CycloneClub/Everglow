using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

[Pipeline(typeof(WCSPipeline))]
public class DarkForestGrass_grass_fore : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
	public override void OnSpawn()
	{
		Texture = ModAsset.DarkForestGrass_grass.Value;
	}

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

		//if (!Grass_FurPipeline.ShouldUpdateRenderTarget)
		//{
		//	Vector2 deltaPos = Grass_FurPipeline.TotalMovedPosition;
		//	int rectangleX = (int)(Main.screenPosition - deltaPos).X;
		//	int rectangleY = (int)(Main.screenPosition - deltaPos).Y;
		//	Rectangle checkRectangle = new Rectangle(rectangleX, rectangleY, Main.screenWidth - 8, Main.screenHeight - 8);
		//	if (checkRectangle.Contains((int)Position.X, (int)Position.Y))
		//	{
		//		return;
		//	}
		//}
		List<Vertex2D> bars = new List<Vertex2D>();
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
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}

	public void AddDrawingFace(List<Vertex2D> bars, Vector2 Direction, Vector2 Position, float length = 1, Vector2 offset = default(Vector2))
	{
		Vector2 drawCenter = Position + new Vector2(8) + offset;
		Vector2 directionN = Vector2.Normalize(Direction);
		float rot = -MathF.Asin(Vector3.Cross(new Vector3(directionN, 0), new Vector3(0, -1, 0)).Z);
		float cos = Vector2.Dot(directionN, new Vector2(0, -1));
		if (cos < 0)
		{
			rot = MathHelper.Pi - rot;
		}
		float dLength = 16 / 52f;
		Vector2 point0 = drawCenter + new Vector2(-8 * length, -8).RotatedBy(rot) + Direction * 8;
		Vector2 point1 = drawCenter + new Vector2(8 * length, -8).RotatedBy(rot) + Direction * 8;
		Vector2 point2 = drawCenter + new Vector2(-8 * length, 8).RotatedBy(rot) + Direction * 12;
		Vector2 point3 = drawCenter + new Vector2(8 * length, 8).RotatedBy(rot) + Direction * 12;

		Color color0 = Lighting.GetColor(point0.ToTileCoordinates());
		Color color1 = Lighting.GetColor(point1.ToTileCoordinates());
		Color color2 = Lighting.GetColor(point2.ToTileCoordinates());
		Color color3 = Lighting.GetColor(point3.ToTileCoordinates());

		bars.Add(point0, color0, new Vector3(18 * style / 52f, 0, 0));
		bars.Add(point1, color1, new Vector3(dLength + 18 * style / 52f, 0, 0));
		bars.Add(point3, color3, new Vector3(dLength + 18 * style / 52f, 1, 0));

		bars.Add(point2, color2, new Vector3(18 * style / 52f, 1, 0));
		bars.Add(point0, color0, new Vector3(18 * style / 52f, 0, 0));
		bars.Add(point3, color3, new Vector3(dLength + 18 * style / 52f, 1, 0));
	}
}
