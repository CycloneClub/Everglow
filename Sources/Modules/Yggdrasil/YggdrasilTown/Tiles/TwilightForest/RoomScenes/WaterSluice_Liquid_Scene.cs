using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class WaterSluice_Liquid_Scene : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public bool FlipHorizontally(int i, int j)
	{
		int leftSolid = 0;
		int rightSolid = 0;
		for (int x = 1; x < 15; x++)
		{
			Tile tileLeft = TileUtils.SafeGetTile(i - x, j);
			Tile tileRight = TileUtils.SafeGetTile(i + x, j);
			if (tileLeft.HasTile && tileLeft.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				leftSolid++;
			}
			if (tileRight.HasTile && tileRight.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				rightSolid++;
			}
		}
		if (rightSolid > leftSolid)
		{
			return false;
		}
		return true;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		bool flipH = FlipHorizontally(OriginTilePos.X, OriginTilePos.Y);
		int Direction = 1;
		if (flipH)
		{
			Direction = -1;
		}
		Vector2 liquidSurfacePos = new Vector2(OriginTilePos.X + 18 * Direction, OriginTilePos.Y + 6).ToWorldCoordinates() + new Vector2(0, 4);
		DrawWaterPierThick(liquidSurfacePos, 0.7f);
		liquidSurfacePos = new Vector2(OriginTilePos.X + 23 * Direction, OriginTilePos.Y + 7).ToWorldCoordinates() + new Vector2(0, 6);
		DrawWaterPierThick(liquidSurfacePos, 0.4f);
		liquidSurfacePos = new Vector2(OriginTilePos.X + 33 * Direction, OriginTilePos.Y + 7).ToWorldCoordinates() + new Vector2(1 * Direction, 0);
		DrawWaterPierThin(liquidSurfacePos, 0.1f);

		// liquidSurfacePos = new Vector2(OriginTilePos.X + 34 * Direction, OriginTilePos.Y + 7).ToWorldCoordinates() + new Vector2(9 * Direction, 0);
		// DrawWaterPierThin(liquidSurfacePos, 0.8f);
		// liquidSurfacePos = new Vector2(OriginTilePos.X + 32 * Direction, OriginTilePos.Y + 7).ToWorldCoordinates() + new Vector2(-7 * Direction, 0);
		// DrawWaterPierThin(liquidSurfacePos, 0.5f);

		// liquidSurfacePos = new Vector2(OriginTilePos.X + 33 * Direction, OriginTilePos.Y + 7).ToWorldCoordinates() + new Vector2(12 * Direction, 0);
		// DrawWaterPierThinBack(liquidSurfacePos, 0.2f);
		// liquidSurfacePos = new Vector2(OriginTilePos.X + 33 * Direction, OriginTilePos.Y + 7).ToWorldCoordinates() + new Vector2(-10 * Direction, 0);
		// DrawWaterPierThinBack(liquidSurfacePos, 0.9f);
		liquidSurfacePos = new Vector2(OriginTilePos.X + 6 * Direction, OriginTilePos.Y + 9).ToWorldCoordinates() + new Vector2(-2 * Direction, 0);
		DrawWaterFallFlow(liquidSurfacePos, 0.6f);
	}

	public void DrawWaterPierThinBack(Vector2 worldPos, float offsetY = 0)
	{
		// Texture2D liquidTex = ModAsset.WaterSluice_Scene_WaterPier_content.Value;
		// Texture2D liquidTex_side = ModAsset.WaterSluice_Scene_WaterPier_side.Value;
		// float timeValue = -(float)Main.time / 120f;
		// List<Vertex2D> bars = new List<Vertex2D>();
		// List<Vertex2D> bars_side = new List<Vertex2D>();
		// List<Vertex2D> bars_wave = new List<Vertex2D>();
		// float fade0 = 0.3f;
		// float fade = 0.1f;
		// float fade2 = 0.002f;
		// float waterWidth = 3;
		// for (int i = 0; i < 300; i++)
		// {
		// Vector2 drawPos = worldPos + new Vector2(0, i * 2);
		// var tile = TileUtils.SafeGetTile(drawPos.ToTileCoordinates());
		// float liquidCut = (16 - drawPos.Y % 16) * 16f;
		// if ((Collision.IsWorldPointSolid(drawPos) && !Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount > liquidCut)
		// {
		// GenerateWaterPierDust(worldPos, drawPos, 3, 0.3f, 0.25f, 0.1f);
		// break;
		// }
		// float coordY = timeValue + MathF.Pow(i / 4f + 6, 0.5f) * 0.14f + offsetY;
		// AddVertex(bars, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY, 0), fade);
		// AddVertex(bars, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY, 0), fade);

		// AddVertex(bars_wave, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY * 2.5f, 0), fade2);
		// AddVertex(bars_wave, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY * 2.5f, 0), fade2);

		// AddVertex(bars_side, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY, 0), fade0);
		// AddVertex(bars_side, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY, 0), fade0);
		// }
		// Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleStrip);
		// Ins.Batch.Draw(Commons.ModAsset.Noise_flame_2_pure.Value, bars_wave, PrimitiveType.TriangleStrip);
		// Ins.Batch.Draw(liquidTex_side, bars_side, PrimitiveType.TriangleStrip);
		Texture2D liquidTex = ModAsset.WaterSluice_Scene_WaterPier_thin.Value;
		float timeValue = -(float)Main.time / 120f;
		List<Vertex2D> bars = new List<Vertex2D>();
		float fade = 0.1f;
		for (int i = 0; i < 300; i++)
		{
			Vector2 drawPos = worldPos + new Vector2(0, i * 2);
			var tile = TileUtils.SafeGetTile(drawPos.ToTileCoordinates());
			float liquidCut = (16 - drawPos.Y % 16) * 16f;
			float waterWidth = 3;
			if ((Collision.IsWorldPointSolid(drawPos) && !Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount > liquidCut)
			{
				GenerateWaterPierDust(worldPos, drawPos, 3, 0.3f, 0.25f, 0.1f);
				break;
			}
			float coordY = timeValue + MathF.Pow(i / 4f + 6, 0.5f) * 0.14f + offsetY;
			AddVertex(bars, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY, 0), fade);
			AddVertex(bars, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY, 0), fade);
		}
		Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleStrip);
	}

	public void DrawWaterPierThin(Vector2 worldPos, float offsetY = 0)
	{
		Texture2D liquidTex = ModAsset.WaterSluice_Scene_WaterPier_thin_batch.Value;
		float timeValue = -(float)Main.time / 120f;
		List<Vertex2D> bars = new List<Vertex2D>();
		float fade = 0.3f;
		for (int i = 0; i < 300; i++)
		{
			Vector2 drawPos = worldPos + new Vector2(0, i * 2);
			var tile = TileUtils.SafeGetTile(drawPos.ToTileCoordinates());
			float liquidCut = (16 - drawPos.Y % 16) * 16f;
			float waterWidth = 29;
			if ((Collision.IsWorldPointSolid(drawPos) && !Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount > liquidCut)
			{
				Vector2 offsetWorld = new Vector2(-3, 0);
				GenerateWaterPierDust(worldPos + new Vector2(26, 0) + offsetWorld, drawPos + new Vector2(26, 0), 3, 1f, 0.25f, 0.1f);
				GenerateWaterPierDust(worldPos + new Vector2(10, 0) + offsetWorld, drawPos + new Vector2(10, 0), 3, 0.3f, 0.25f, 0.1f);
				GenerateWaterPierDust(worldPos + offsetWorld, drawPos, 3, 1f, 0.25f, 0.1f);
				GenerateWaterPierDust(worldPos + new Vector2(-10, 0) + offsetWorld, drawPos + new Vector2(-10, 0), 3, 0.3f, 0.25f, 0.1f);
				GenerateWaterPierDust(worldPos + new Vector2(-26, 0) + offsetWorld, drawPos + new Vector2(-26, 0), 3, 1f, 0.25f, 0.1f);
				break;
			}
			float coordY = timeValue + MathF.Pow(i / 4f + 6, 0.5f) * 0.14f + offsetY;
			AddVertex(bars, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY, 0), fade);
			AddVertex(bars, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY, 0), fade);
		}
		Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleStrip);
	}

	public void DrawWaterPierThick(Vector2 worldPos, float offsetY = 0)
	{
		Texture2D liquidTex = ModAsset.WaterSluice_Scene_WaterPier_content.Value;
		Texture2D liquidTex_side = ModAsset.WaterSluice_Scene_WaterPier_side.Value;
		float timeValue = -(float)Main.time / 120f;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_side = new List<Vertex2D>();
		List<Vertex2D> bars_wave = new List<Vertex2D>();
		float fade = 0.3f;
		float fade2 = 0.06f;
		float waterWidth = 10;
		for (int i = 0; i < 300; i++)
		{
			Vector2 drawPos = worldPos + new Vector2(0, i * 2);
			var tile = TileUtils.SafeGetTile(drawPos.ToTileCoordinates());
			float liquidCut = (16 - drawPos.Y % 16) * 16f;
			if ((Collision.IsWorldPointSolid(drawPos) && !Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount > liquidCut)
			{
				GenerateWaterPierDust(worldPos, drawPos, 10, 1f, 0.5f, 0.5f, 1f, 1f);
				break;
			}
			float coordY = timeValue + MathF.Pow(i / 4f + 6, 0.5f) * 0.14f + offsetY;
			AddVertex(bars, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY, 0), fade);
			AddVertex(bars, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY, 0), fade);

			AddVertex(bars_wave, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY * 2.5f, 0), fade2);
			AddVertex(bars_wave, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY * 2.5f, 0), fade2);

			AddVertex(bars_side, drawPos + new Vector2(-waterWidth, 0), new Vector3(0, coordY, 0));
			AddVertex(bars_side, drawPos + new Vector2(waterWidth, 0), new Vector3(1, coordY, 0));
		}
		Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_2_pure.Value, bars_wave, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(liquidTex_side, bars_side, PrimitiveType.TriangleStrip);
	}

	public void DrawWaterFallFlow(Vector2 worldPos, float offsetY = 0)
	{
		Texture2D liquidTex = ModAsset.WaterSluice_Scene_WaterPier_content.Value;
		Texture2D liquidTex_side = ModAsset.WaterSluice_Scene_Waterfall_side.Value;
		float timeValue = -(float)Main.time / 120f;
		bool flipH = FlipHorizontally(OriginTilePos.X, OriginTilePos.Y);
		int Direction = 1;
		if (flipH)
		{
			Direction = -1;
		}

		// Legacy
		// for (int i = 0; i < 14; i++)
		// {
		// List<Vertex2D> bars = new List<Vertex2D>();
		// List<Vertex2D> bars_side = new List<Vertex2D>();
		// List<Vertex2D> bars_wave = new List<Vertex2D>();
		// List<Vertex2D> bars_wave2 = new List<Vertex2D>();
		// float fade = 0.3f;
		// float fade2 = 0.06f;
		// float waterWidth = 2;
		// for (int j = 0; j < 300; j++)
		// {
		// Vector2 drawPos = worldPos + new Vector2(i * 4 * Direction, j * 2);
		// Vector2 offset0 = new Vector2(GetMove(i, j, 7) * Direction, 0);
		// Vector2 offset1 = new Vector2(GetMove(i + 1, j, 7) * Direction, 0);
		// if (Direction == -1)
		// {
		// (offset0, offset1) = (offset1, offset0);
		// }
		// var tile = TileUtils.SafeGetTile(drawPos.ToTileCoordinates());
		// float liquidCut = (16 - drawPos.Y % 16) * 16f;
		// if ((Collision.IsWorldPointSolid(drawPos) && !Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount > liquidCut)
		// {
		// GenerateWaterfallDust(worldPos + new Vector2(i * 4 * Direction, 0), drawPos + offset0, 4, 0.5f, i, 0.5f, 0.15f, 0.1f, 0.5f);
		// break;
		// }
		// float coordY = timeValue + MathF.Pow(j / 4f + 0, 0.5f) * 0.14f + offsetY;

		// AddVertex(bars, drawPos + offset0 + new Vector2(-waterWidth, 0), new Vector3(0.5f, 0, 0), fade);
		// AddVertex(bars, drawPos + offset1 + new Vector2(waterWidth, 0), new Vector3(0.5f, 0, 0), fade);

		// AddVertex(bars_wave, drawPos + offset0 + new Vector2(-waterWidth, 0), new Vector3(i / 14f, coordY * 2.5f, 0), fade2 - j * 0.0007f);
		// AddVertex(bars_wave, drawPos + offset1 + new Vector2(waterWidth, 0), new Vector3((i + 1) / 14f, coordY * 2.5f, 0), fade2 - j * 0.0007f);

		// AddVertexTransparent(bars_wave2, drawPos + offset0 + new Vector2(-waterWidth, 0), new Vector3(i / 14f, coordY * 1f, 0), 0.2f);
		// AddVertexTransparent(bars_wave2, drawPos + offset1 + new Vector2(waterWidth, 0), new Vector3((i + 1) / 14f, coordY * 1f, 0), 0.2f);

		// if (i == 0 || i == 13)
		// {
		// AddVertex(bars_side, drawPos + offset0 + new Vector2(-waterWidth, 0), new Vector3(i / 14f, 0.6f, 0), fade);
		// AddVertex(bars_side, drawPos + offset1 + new Vector2(waterWidth, 0), new Vector3((i + 1) / 14f, 0.6f, 0), fade);
		// }
		// }
		// if (bars.Count > 2)
		// {
		// Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleStrip);
		// Ins.Batch.Draw(Commons.ModAsset.Noise_flame_2_pure.Value, bars_wave, PrimitiveType.TriangleStrip);
		// Ins.Batch.Draw(Commons.ModAsset.Noise_WaterFallWave.Value, bars_wave2, PrimitiveType.TriangleStrip);
		// }
		// if (bars_side.Count > 2)
		// {
		// Ins.Batch.Draw(liquidTex_side, bars_side, PrimitiveType.TriangleStrip);
		// }
		// }
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_side = new List<Vertex2D>();
		List<Vertex2D> bars_wave = new List<Vertex2D>();
		List<Vertex2D> bars_wave2 = new List<Vertex2D>();
		float fade = 0.3f;
		float fade2 = 0.06f;
		float waterWidth = 2;
		for (int j = 0; j < 300; j++)
		{
			Vector2 drawPos0 = worldPos + new Vector2(14 * 4 * Direction, j * 2);
			Vector2 drawPos1 = worldPos + new Vector2(-1 * 4 * Direction, j * 2);
			Vector2 offset0 = new Vector2(GetMove(14, j, 7) * Direction, 0);
			Vector2 offset1 = new Vector2(GetMove(0, j, 7) * Direction, 0);
			if (Direction == -1)
			{
				(offset0, offset1) = (offset1, offset0);
				(drawPos0, drawPos1) = (drawPos1, drawPos0);
			}
			var tile = TileUtils.SafeGetTile(drawPos0.ToTileCoordinates());
			float liquidCut = (16 - drawPos0.Y % 16) * 16f;
			if ((Collision.IsWorldPointSolid(drawPos0) && !Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount > liquidCut)
			{
				if (Direction == -1)
				{
					GenerateWaterfallDust(worldPos + new Vector2(0, 0), drawPos0 + offset0, 4, 0.5f, 7, 0.5f, 0f, 1f, 0f);
					GenerateWaterfallDust(worldPos + new Vector2(0, 0), drawPos0 + offset0, 4, 0.5f, 7, 0.5f, 0.5f, 1f, 1f);
				}
				else
				{
					GenerateWaterfallDust(worldPos + new Vector2(0, 0), drawPos1 + offset1 + new Vector2(2, 0), 4, 0.5f, 7, 0.5f, 0f, 1f, 0f);
					GenerateWaterfallDust(worldPos + new Vector2(0, 0), drawPos1 + offset1 + new Vector2(2, 0), 4, 0.5f, 7, 0.5f, 0.5f, 1f, 1f);
				}
				break;
			}
			float coordY = timeValue + MathF.Pow(j / 4f + 0, 0.5f) * 0.14f + offsetY;

			AddVertex(bars, drawPos0 + offset0 + new Vector2(-waterWidth, 0), new Vector3(0.5f, 0, 0), fade);
			AddVertex(bars, drawPos1 + offset1 + new Vector2(waterWidth, 0), new Vector3(0.5f, 0, 0), fade);

			AddVertex(bars_wave, drawPos0 + offset0 + new Vector2(-waterWidth, 0), new Vector3(0, coordY * 2.5f, 0), fade2 - j * 0.0007f);
			AddVertex(bars_wave, drawPos1 + offset1 + new Vector2(waterWidth, 0), new Vector3(1, coordY * 2.5f, 0), fade2 - j * 0.0007f);

			AddVertexTransparent(bars_wave2, drawPos0 + offset0 + new Vector2(-waterWidth, 0), new Vector3(0, coordY * 1f, 0), 0.2f);
			AddVertexTransparent(bars_wave2, drawPos1 + offset1 + new Vector2(waterWidth, 0), new Vector3(1, coordY * 1f, 0), 0.2f);

			AddVertex(bars_side, drawPos0 + offset0 + new Vector2(-waterWidth, 0), new Vector3(0, 0.6f, 0), fade);
			AddVertex(bars_side, drawPos1 + offset1 + new Vector2(waterWidth, 0), new Vector3(1, 0.6f, 0), fade);
		}
		if (bars.Count > 2)
		{
			Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleStrip);
			Ins.Batch.Draw(Commons.ModAsset.Noise_flame_2_pure.Value, bars_wave, PrimitiveType.TriangleStrip);
			Ins.Batch.Draw(Commons.ModAsset.Noise_WaterFallWave.Value, bars_wave2, PrimitiveType.TriangleStrip);
		}
		if (bars_side.Count > 2)
		{
			Ins.Batch.Draw(liquidTex_side, bars_side, PrimitiveType.TriangleStrip);
		}
	}

	public float GetMove(int i, int j, float middle)
	{
		return (middle - i) * MathF.Pow(j, 0.5f) * 0.15f;
	}

	public void GenerateWaterPierDust(Vector2 pierStartPos, Vector2 pierEndPos, float width, float fade, float fallingChance = 0.25f, float splashDustChance = 0.25f, float splashDustShockWaveChance = 0.5f, float bubbleChance = 0.5f)
	{
		if (!Main.gamePaused)
		{
			// splash dust
			if (Main.rand.NextFloat() < splashDustChance)
			{
				Vector2 pos = new Vector2(pierEndPos.X + Main.rand.NextFloat(-width, width), pierEndPos.Y + Main.rand.NextFloat(-4, 0));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_Dust()
				{
					Position = pos,
					Velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-2, -1)),
					Fade = fade,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = pos.Y + 4,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
					Scale = Main.rand.NextFloat(0.25f, 0.4f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
				if (Main.rand.NextFloat() < bubbleChance)
				{
					pos = new Vector2(pierEndPos.X + Main.rand.NextFloat(-width, width), pierEndPos.Y + 4);
					Dust dust2 = Dust.NewDustPerfect(pos, ModContent.DustType<WaterSluiceBubble>());
					dust2.velocity *= 0.3f;
					dust2.velocity.Y = Main.rand.NextFloat(0.5f, 2f);
					dust2.scale = Main.rand.NextFloat(0.35f, 0.75f);
				}
			}

			// shock wave
			if (Main.rand.NextFloat() < splashDustShockWaveChance)
			{
				Vector2 pos = new Vector2(pierEndPos.X + Main.rand.NextFloat(-width - 1, width + 1), pierEndPos.Y + Main.rand.NextFloat(-4, 0));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_shockwave()
				{
					Position = pos,
					Fade = fade * Main.rand.NextFloat(0.75f, 1),
					Timer = 0,
					MaxTime = 12,
					Frame = new Rectangle(0, Main.rand.Next(8) * 16, 8, 16),
					Scale = width / 12f * Main.rand.NextFloat(0.75f, 1.25f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
			}

			// falling dust
			if (Main.rand.NextFloat() < fallingChance)
			{
				Vector2 pos = new Vector2(pierStartPos.X + Main.rand.NextFloat(-width, -(width - 2)), pierStartPos.Y + Main.rand.NextFloat(0, 0));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_Dust()
				{
					Position = pos,
					Velocity = new Vector2(Main.rand.NextFloat(-0.2f, 0.1f), Main.rand.NextFloat(2, 2.5f)),
					Fade = fade,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = pierEndPos.Y + 4,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.6f) * width / 12f,
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
			}
			if (Main.rand.NextFloat() < fallingChance)
			{
				Vector2 pos = new Vector2(pierStartPos.X + Main.rand.NextFloat(width - 2, width), pierStartPos.Y + Main.rand.NextFloat(0, 0));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_Dust()
				{
					Position = pos,
					Velocity = new Vector2(Main.rand.NextFloat(-0.1f, 0.2f), Main.rand.NextFloat(2, 2.5f)),
					Fade = fade,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = pierEndPos.Y + 4,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.6f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
			}
		}
	}

	public void GenerateWaterfallDust(Vector2 waterfallStartPos, Vector2 waterfallEndPos, float width, float fade, float i, float fallingChance = 0.25f, float splashDustChance = 0.25f, float splashDustShockWaveChance = 0.5f, float bubbleChance = 0.5f)
	{
		if (!Main.gamePaused)
		{
			// splash dust
			bool flipH = FlipHorizontally(OriginTilePos.X, OriginTilePos.Y);
			int Direction = 1;
			if (flipH)
			{
				Direction = -1;
			}
			if (Main.rand.NextFloat() < splashDustChance)
			{
				Vector2 pos = new Vector2(waterfallEndPos.X + Main.rand.NextFloat(0, 40) * Direction, waterfallEndPos.Y + Main.rand.NextFloat(-4, 0));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_Dust()
				{
					Position = pos,
					Velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-3, -1)),
					Fade = fade * Main.rand.NextFloat(0.75f, 1.5f),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = pos.Y + 4,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
					Scale = Main.rand.NextFloat(0.25f, 0.4f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
				if (Main.rand.NextFloat() < bubbleChance)
				{
					pos = new Vector2(waterfallEndPos.X + Main.rand.NextFloat(0, 40) * Direction, waterfallEndPos.Y + 4);
					Dust dust2 = Dust.NewDustPerfect(pos, ModContent.DustType<WaterSluiceBubble>());
					dust2.velocity *= 0.3f;
					dust2.velocity.Y = Main.rand.NextFloat(0.5f, 2f);
					dust2.scale = Main.rand.NextFloat(0.35f, 0.75f);
				}
			}

			// shock wave
			if (Main.rand.NextFloat() < splashDustShockWaveChance)
			{
				Vector2 pos = new Vector2(waterfallEndPos.X + Main.rand.NextFloat(0, 40) * Direction, waterfallEndPos.Y + Main.rand.NextFloat(-4, -10));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_shockwave()
				{
					Position = pos,
					Fade = fade,
					Timer = 0,
					MaxTime = 12,
					Frame = new Rectangle(0, Main.rand.Next(8) * 16, 8, 16),
					Scale = Main.rand.NextFloat(0.5f, 1.2f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
			}

			// falling dust
			if (Main.rand.NextFloat() < fallingChance)
			{
				Vector2 pos = new Vector2(waterfallStartPos.X + Main.rand.NextFloat(-2, 2), waterfallStartPos.Y + Main.rand.NextFloat(0, 4));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_Dust()
				{
					Position = pos,
					Velocity = new Vector2(GetMove(0, 20, 7) * Direction * 0.03f, 0),
					Fade = fade,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = waterfallEndPos.Y + 4,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.6f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
			}

			if (Main.rand.NextFloat() < fallingChance)
			{
				Vector2 pos = new Vector2(waterfallStartPos.X + 13 * 4 * Direction + Main.rand.NextFloat(-2, 2), waterfallStartPos.Y + Main.rand.NextFloat(0, 4));
				Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
				var splash = new WaterSluice_Scene_Dust()
				{
					Position = pos,
					Velocity = new Vector2(GetMove(13, 20, 7) * Direction * 0.03f, 0),
					Fade = fade,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = waterfallEndPos.Y + 4,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10 + Main.rand.Next(3) * 10, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.6f),
					Active = true,
					Visible = true,
				};
				dust.customData = splash;
			}
		}
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 worldPos, Vector3 coord, float mulColor = 1)
	{
		bars.Add(worldPos, Lighting.GetColor(worldPos.ToTileCoordinates()) * mulColor, coord);
	}

	public void AddVertexTransparent(List<Vertex2D> bars, Vector2 worldPos, Vector3 coord, float mulColor = 1)
	{
		Color drawColor = Lighting.GetColor(worldPos.ToTileCoordinates()) * mulColor;
		drawColor.A = 0;
		bars.Add(worldPos, drawColor, coord);
	}

	public float GetRandomWave(float x)
	{
		float value = 0;
		float timeValue = (float)Main.time * 0.06f;
		for (int i = 0; i < 8; i++)
		{
			float rate = MathF.Pow(2, i);
			value += MathF.Sin(x * rate + timeValue * rate * 0.5f) / rate;
		}
		return value;
	}
}
