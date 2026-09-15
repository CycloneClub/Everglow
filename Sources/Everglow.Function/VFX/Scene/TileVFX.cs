using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.Scene;

[Pipeline(typeof(WCSPipeline))]
public abstract class TileVFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public Vector2 Position;
	public Texture2D Texture;
	public Point OriginTilePos;
	public int OriginTileType;
	public int Direction = 1;
	public float MaxDiatanceOutOfScreen = 500;

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
		if (VFXManager.InScreen(checkPos, MaxDiatanceOutOfScreen))
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

	public override void Draw()
	{
		Color lightColor0 = Lighting.GetColor((int)Position.X / 16, (int)Position.Y / 16);
		Color lightColor1 = Lighting.GetColor((int)(Position.X + Texture.Width) / 16, (int)Position.Y / 16);
		Color lightColor2 = Lighting.GetColor((int)Position.X / 16, (int)(Position.Y + Texture.Height) / 16);
		Color lightColor3 = Lighting.GetColor((int)(Position.X + Texture.Width) / 16, (int)(Position.Y + Texture.Height) / 16);

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position, lightColor0, new Vector3(0, 0, 0)),
			new Vertex2D(Position + new Vector2(Texture.Width, 0), lightColor1, new Vector3(1, 0, 0)),

			new Vertex2D(Position + new Vector2(0, Texture.Height), lightColor2, new Vector3(0, 1, 0)),
			new Vertex2D(Position + new Vector2(Texture.Width, Texture.Height), lightColor3, new Vector3(1, 1, 0)),
		};
		if (Direction < 0)
		{
			bars = new List<Vertex2D>()
			{
				new Vertex2D(Position, lightColor0, new Vector3(1, 0, 0)),
				new Vertex2D(Position + new Vector2(Texture.Width, 0), lightColor1, new Vector3(0, 0, 0)),

				new Vertex2D(Position + new Vector2(0, Texture.Height), lightColor2, new Vector3(1, 1, 0)),
				new Vertex2D(Position + new Vector2(Texture.Width, Texture.Height), lightColor3, new Vector3(0, 1, 0)),
			};
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
