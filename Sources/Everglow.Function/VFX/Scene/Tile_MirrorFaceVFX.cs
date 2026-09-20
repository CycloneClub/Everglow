using Everglow.Commons.Enums;
using Everglow.Commons.VFX.Pipelines;
using static Everglow.Commons.VFX.Pipelines.ScreenReflectionPipeline;

namespace Everglow.Commons.VFX.Scene;

[Pipeline(typeof(ScreenReflectionPipeline))]
public abstract class Tile_MirrorFaceVFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Texture2D Texture;
	public Point OriginTilePos;
	public int OriginTileType;
	public float DepthZ = -50;

	public override void OnSpawn()
	{
	}

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
							}
						}
						else
						{
							Active = false;
							SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
						}
					}
					else
					{
						Active = false;
						SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
					}
				}
				else
				{
					Active = false;
					SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
				}
			}
			else
			{
				Active = false;
				SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
			}
		}
		else
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
		}
		Vector2 checkPos = Position + Texture.Size() / 2;
		if (VFXManager.InScreen(checkPos, Math.Max(Texture.Width, Texture.Height + 200)))
		{
			Visible = true;
		}
		else
		{
			Active = false;
			SceneVFXSystem.TilePointHasScene[(OriginTilePos.X, OriginTilePos.Y)] = false;
		}
	}

	public override void Draw()
	{
		Color color = Color.White;
		Main.graphics.GraphicsDevice.Textures[1] = Texture;
		float z = DepthZ;
		List<MirrorFaceVertex> bars = new List<MirrorFaceVertex>()
		{
			new MirrorFaceVertex(new Vector3(Position, z), color, new Vector3(0, 0, 0)),
			new MirrorFaceVertex(new Vector3(Position + new Vector2(Texture.Width, 0), z), color, new Vector3(1, 0, 0)),

			new MirrorFaceVertex(new Vector3(Position + new Vector2(0, Texture.Height), z), color, new Vector3(0, 1, 0)),
			new MirrorFaceVertex(new Vector3(Position + new Vector2(Texture.Width, Texture.Height), z), color, new Vector3(1, 1, 0)),
		};
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}
}
