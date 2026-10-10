using Everglow.Commons.DataStructures;
using Everglow.Myth.TheTusk.Tiles;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class TuskWall_Wave : ModProjectile
{
	public override string Texture => ModAsset.Empty_Mod;

	public List<(Point TileCoord, float Rotation)> WavedTiles = new List<(Point, float)>();

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 280;
		Projectile.alpha = 255;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Vector2 checkDir = TileUtils.GetTopographicGradient(Projectile.Center, 20);
		for (int y = 0; y < 100; y++)
		{
			if (Collision.SolidCollision(Projectile.Center - new Vector2(8), 16, 16))
			{
				break;
			}
			Projectile.position -= checkDir * 10;
			if (y == 99)
			{
				Projectile.active = false;
			}
		}
		Point point = Projectile.Center.ToTileCoordinates();
		for (int i = -30; i < 31; i++)
		{
			for (int j = -30; j < 31; j++)
			{
				if (new Vector2(i, j).Length() <= 30)
				{
					Point tileCoord = point + new Point(i, j);
					if (tileCoord.X >= 20 && tileCoord.X <= Main.maxTilesX - 20 && tileCoord.Y >= 20 && tileCoord.Y <= Main.maxTilesY - 20)
					{
						if (Main.tile[tileCoord].TileType == ModContent.TileType<TuskFlesh>() || Main.tile[tileCoord].TileType == ModContent.TileType<AbTuskFlesh>())
						{
							if (IsBoundaryTile(tileCoord))
							{
								WavedTiles.Add((tileCoord, TileUtils.GetTopographicGradient(tileCoord.ToWorldCoordinates(), 4).ToRotation()));
							}
						}
					}
				}
			}
		}
		Projectile.position -= Projectile.rotation.ToRotationVector2() * 20;
	}

	public bool IsBoundaryTile(Point point)
	{
		if (point.X >= 20 && point.X <= Main.maxTilesX - 20 && point.Y >= 20 && point.Y <= Main.maxTilesY - 20)
		{
			Vector2 worldCoord = point.ToWorldCoordinates();
			int activeTile = 0;
			if (Collision.SolidCollision(worldCoord + new Vector2(0, -16), 0, 0))
			{
				activeTile++;
			}
			if (Collision.SolidCollision(worldCoord + new Vector2(0, 16), 0, 0))
			{
				activeTile++;
			}
			if (Collision.SolidCollision(worldCoord + new Vector2(-16, 0), 0, 0))
			{
				activeTile++;
			}
			if (Collision.SolidCollision(worldCoord + new Vector2(16, 0), 0, 0))
			{
				activeTile++;
			}
			if (activeTile is >= 1 and < 4)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect effect = Commons.ModAsset.Shader2D.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Texture2D texture = ModAsset.TuskFlesh.Value;
		List<Vertex2D> bars = new List<Vertex2D>();
		foreach (var item in WavedTiles)
		{
			Tile tile = Main.tile[item.TileCoord];
			AddVertex(bars, GetWave(item, new Vector2(-8, -8)), new Vector3(tile.TileFrameX / (float)texture.Width, tile.TileFrameY / (float)texture.Height, 0));
			AddVertex(bars, GetWave(item, new Vector2(8, -8)), new Vector3((tile.TileFrameX + 16) / (float)texture.Width, tile.TileFrameY / (float)texture.Height, 0));
			AddVertex(bars, GetWave(item, new Vector2(-8, 8)), new Vector3(tile.TileFrameX / (float)texture.Width, (tile.TileFrameY + 16) / (float)texture.Height, 0));

			AddVertex(bars, GetWave(item, new Vector2(-8, 8)), new Vector3(tile.TileFrameX / (float)texture.Width, (tile.TileFrameY + 16) / (float)texture.Height, 0));
			AddVertex(bars, GetWave(item, new Vector2(8, -8)), new Vector3((tile.TileFrameX + 16) / (float)texture.Width, tile.TileFrameY / (float)texture.Height, 0));
			AddVertex(bars, GetWave(item, new Vector2(8, 8)), new Vector3((tile.TileFrameX + 16) / (float)texture.Width, (tile.TileFrameY + 16) / (float)texture.Height, 0));
		}
		Main.graphics.graphicsDevice.Textures[0] = texture;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public Vector2 GetWave((Point TileCoord, float Rotation) item, Vector2 offset)
	{
		Vector2 worldCoord = item.TileCoord.ToWorldCoordinates() + offset;
		float distance = (worldCoord - Projectile.Center).Length();
		worldCoord -= offset;
		float wave = MathF.Sin(MathF.Pow(distance, 0.7f) * 0.45f + Projectile.timeLeft * 0.4f) + 1;
		wave *= 15;
		float distanceFade = (400 - distance) / 400f;
		distanceFade = Math.Max(distanceFade, 0);
		distanceFade = MathF.Pow(distanceFade, 0.7f);
		wave *= distanceFade;
		float timeFade = 140 - Math.Abs(Projectile.timeLeft - 140);
		timeFade = Math.Min(timeFade, 30) / 30f;
		wave *= timeFade;
		Vector2 direction = item.Rotation.ToRotationVector2();
		float cos = Vector2.Dot(offset, direction) / offset.Length();
		cos = Math.Max(cos, 0);
		offset += direction * cos * wave;
		return worldCoord + offset;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos, Vector3 coords, bool transparent = false)
	{
		bars.Add(pos, Lighting.GetColor(pos.ToTileCoordinates()) * (transparent ? 0 : 1), coords);
	}
}
