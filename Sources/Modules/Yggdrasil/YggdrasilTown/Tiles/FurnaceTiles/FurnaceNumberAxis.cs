using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

[Pipeline(typeof(WCSPipeline))]
public class FurnaceNumberAxis : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public override void Update()
	{
		if (OriginTilePos.X > 0 && OriginTilePos.X < Main.maxTilesX)
		{
			if (OriginTilePos.Y > 0 && OriginTilePos.Y < Main.maxTilesY)
			{
				Tile tile = Main.tile[OriginTilePos.X, OriginTilePos.Y];
				if (tile != null)
				{
					if (TileLoader.GetTile(tile.TileType) is ISceneTile)
					{
						if (tile.TileType == OriginTileType)
						{
							if (!tile.HasTile)
							{
								Active = false;
								SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
								return;
							}
						}
						else
						{
							Active = false;
							SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
							return;
						}
					}
					else
					{
						Active = false;
						SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
						return;
					}
				}
				else
				{
					Active = false;
					SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
					return;
				}
			}
			else
			{
				Active = false;
				SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
				return;
			}
		}
		else
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		Vector2 checkPos = Position;
		if (VFXManager.InScreen(checkPos, 1500))
		{
			Visible = true;
		}
		else
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			return;
		}
		Score = YggdrasilTownFurnaceSystem.CurrentScore;
		Timer++;
		if (Timer >= 3628800)
		{
			Timer = 0;
		}
		if (Timer % 6 == 0)
		{
			string scoreText = Score.ToString();
			for (int i = 0; i < 10; i++)
			{
				int value = 0;
				if (i >= 10 - scoreText.Length)
				{
					value = GetDigitAtPosition(Score, 10 - i);
					if (value <= 0)
					{
						value = 0;
					}
				}
				if (NumberSlotFrameY[i] != value * 3)
				{
					NumberSlotFrameY[i]++;
				}
				if (NumberSlotFrameY[i] > 29)
				{
					NumberSlotFrameY[i] = 0;
				}
			}
		}
	}

	public int Score = 0;

	public int[] NumberSlotFrameY = new int[10];

	public int Timer = 0;

	public override void OnSpawn()
	{
		Texture = ModAsset.FurnaceNumberAxis.Value;
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();

		Vector2 frameSize = new Vector2(36, 38);
		for (int i = 0; i < 10; i++)
		{
			int value = NumberSlotFrameY[i];
			Vector2 frameXY = new Vector2(0, value * frameSize.Y);
			var drawPos = Position + new Vector2(i * 36 + 22, 25);
			var topLeft = drawPos + new Vector2(-frameSize.X, -frameSize.Y) * 0.5f;
			var topRight = drawPos + new Vector2(frameSize.X, -frameSize.Y) * 0.5f;
			var bottomLeft = drawPos + new Vector2(-frameSize.X, frameSize.Y) * 0.5f;
			var bottomRight = drawPos + new Vector2(frameSize.X, frameSize.Y) * 0.5f;
			bars.Add(topLeft, GetColor(topLeft), new Vector3(frameXY / Texture.Size(), 0));
			bars.Add(topRight, GetColor(topRight), new Vector3(new Vector2(frameXY.X + frameSize.X, frameXY.Y) / Texture.Size(), 0));
			bars.Add(bottomRight, GetColor(bottomRight), new Vector3(new Vector2(frameXY.X + frameSize.X, frameXY.Y + frameSize.Y) / Texture.Size(), 0));

			bars.Add(topLeft, GetColor(topLeft), new Vector3(frameXY / Texture.Size(), 0));
			bars.Add(bottomRight, GetColor(bottomRight), new Vector3(new Vector2(frameXY.X + frameSize.X, frameXY.Y + frameSize.Y) / Texture.Size(), 0));
			bars.Add(bottomLeft, GetColor(bottomLeft), new Vector3(new Vector2(frameXY.X, frameXY.Y + frameSize.Y) / Texture.Size(), 0));

			frameXY = new Vector2(36, value * frameSize.Y);
			float reflectValue = 2f;
			bars.Add(topLeft, GetColor(topLeft) * reflectValue, new Vector3(frameXY / Texture.Size(), 0));
			bars.Add(topRight, GetColor(topRight) * reflectValue, new Vector3(new Vector2(frameXY.X + frameSize.X, frameXY.Y) / Texture.Size(), 0));
			bars.Add(bottomRight, GetColor(bottomRight) * reflectValue, new Vector3(new Vector2(frameXY.X + frameSize.X, frameXY.Y + frameSize.Y) / Texture.Size(), 0));

			bars.Add(topLeft, GetColor(topLeft) * reflectValue, new Vector3(frameXY / Texture.Size(), 0));
			bars.Add(bottomRight, GetColor(bottomRight) * reflectValue, new Vector3(new Vector2(frameXY.X + frameSize.X, frameXY.Y + frameSize.Y) / Texture.Size(), 0));
			bars.Add(bottomLeft, GetColor(bottomLeft) * reflectValue, new Vector3(new Vector2(frameXY.X, frameXY.Y + frameSize.Y) / Texture.Size(), 0));
		}

		if (bars.Count <= 0)
		{
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));

			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}

	public Color GetColor(Vector2 drawPos)
	{
		return Lighting.GetColor(drawPos.ToTileCoordinates());
	}

	public int GetDigitAtPosition(int number, int Position)
	{
		string numberStr = number.ToString();
		if (Position < 1 || Position > numberStr.Length)
		{
			return -1;
		}
		char digitChar = numberStr[numberStr.Length - Position];
		return (int)char.GetNumericValue(digitChar);
	}
}
