using Everglow.Commons.VFX.Scene;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class Union_Y_Stairs_Front : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
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
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.Union_Y_Stairs_Front.Value;
	}

	public override void Draw()
	{
		List<Vertex2D> bars = new List<Vertex2D>();
		float width = 38;
		float height = 25;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Color lightColor0 = Lighting.GetColor((int)Position.X / 16 + i, (int)Position.Y / 16 + j);
				Color lightColor1 = Lighting.GetColor((int)Position.X / 16 + i + 1, (int)Position.Y / 16 + j);
				Color lightColor2 = Lighting.GetColor((int)Position.X / 16 + i, (int)Position.Y / 16 + j + 1);
				Color lightColor3 = Lighting.GetColor((int)Position.X / 16 + i + 1, (int)Position.Y / 16 + j + 1);

				bars.Add(Position + new Vector2(i, j) * 16, lightColor0, new Vector3(i / width, j / height, 0));
				bars.Add(Position + new Vector2(i + 1, j) * 16, lightColor1, new Vector3((i + 1) / width, j / height, 0));
				bars.Add(Position + new Vector2(i, j + 1) * 16, lightColor2, new Vector3(i / width, (j + 1) / height, 0));

				bars.Add(Position + new Vector2(i, j + 1) * 16, lightColor2, new Vector3(i / width, (j + 1) / height, 0));
				bars.Add(Position + new Vector2(i + 1, j) * 16, lightColor1, new Vector3((i + 1) / width, j / height, 0));
				bars.Add(Position + new Vector2(i + 1, j + 1) * 16, lightColor3, new Vector3((i + 1) / width, (j + 1) / height, 0));
			}
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
}
