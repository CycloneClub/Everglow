using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class BloodChurch_Liquid_Scene : TileVFX
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

	public List<Rectangle> LiquidAreas = new List<Rectangle>();

	public override void Update()
	{
		if (LiquidAreas.Count <= 0)
		{
			bool flipH = FlipHorizontally(OriginTilePos.X, OriginTilePos.Y);
			int Direction = 1;
			int offsetX = 0;
			if (flipH)
			{
				Direction = -1;
				offsetX = -176;
			}
			LiquidAreas.Add(new Rectangle((OriginTilePos.X + 14 * Direction) * 16 + offsetX, (OriginTilePos.Y + 18) * 16, 192, 16));
		}
		foreach (Player player in Main.player)
		{
			if (player != null && player.active && player.velocity.Length() > 1)
			{
				foreach (Rectangle liquid in LiquidAreas)
				{
					if (player.Hitbox.Intersects(liquid))
					{
						GenerateWaterSplash(liquid, player.Hitbox);
					}
				}
			}
		}
		base.Update();
	}

	public void GenerateWaterSplash(Rectangle liquidBox, Rectangle entityBox)
	{
		int splashStart = entityBox.X;
		int splashEnd = entityBox.X + entityBox.Width;
		splashStart = Math.Max(splashStart, liquidBox.X);
		splashEnd = Math.Min(splashEnd, liquidBox.X + liquidBox.Width);
		float length = splashEnd - splashStart;
		for (int x = 0; x < length / 6; x++)
		{
			Vector2 pos = new Vector2(splashStart + Main.rand.NextFloat(length), liquidBox.Y);
			Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<GenerateSplash>(), Vector2.zeroVector);
			var splash = new BloodChurch_Scene_FakeLiquid_Dust()
			{
				Position = pos,
				Velocity = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, -1)),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Timer = 0,
				MaxTime = 150,
				MaxPosY = pos.Y + 8,
				Frame = new Rectangle(Main.rand.Next(4) * 10, 10, 10, 10),
				Scale = Main.rand.NextFloat(0.35f, 0.6f),
				Active = true,
				Visible = true,
			};
			dust.customData = splash;
		}
	}

	public override void Draw()
	{
		bool flipH = FlipHorizontally(OriginTilePos.X, OriginTilePos.Y);
		int Direction = 1;
		if (flipH)
		{
			Direction = -1;
		}

		// Fountain blood liquid
		Texture2D liquidTex = ModAsset.BloodChurch_Scene_FakeLiquid_dark.Value;
		Texture2D liquidTex_highlight = ModAsset.BloodChurch_Scene_FakeLiquid.Value;
		float timeValue = -(float)Main.time / 240f;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_highlight = new List<Vertex2D>();
		Vector2 liquidSurfacePos = new Vector2(OriginTilePos.X + 13 * Direction, OriginTilePos.Y + 18).ToWorldCoordinates();

		for (int i = 0; i < 26; i++)
		{
			Vector2 drawPos = liquidSurfacePos + new Vector2(i * 8 * Direction, 4);
			float coordX0 = i / 34f + timeValue * 0.4f;
			float coordX1 = (i + 1 * Direction) / 34f + timeValue * 0.4f;

			Color waterEnvLight0 = Lighting.GetColor(drawPos.ToTileCoordinates()) * 0.75f;
			Color waterEnvLight1 = Lighting.GetColor((drawPos + new Vector2(8, 0)).ToTileCoordinates()) * 0.75f;
			float pureRed0 = (waterEnvLight0.R - Math.Max(waterEnvLight0.G, waterEnvLight0.B)) / 255f;
			float pureRed1 = (waterEnvLight1.R - Math.Max(waterEnvLight1.G, waterEnvLight1.B)) / 255f;
			pureRed0 = MathF.Pow(pureRed0, 5) * 2;
			pureRed1 = MathF.Pow(pureRed1, 5) * 2;
			int toCenter = (int)Math.Abs(12.5 - i);
			if (i > 5 && i < 21)
			{
				pureRed0 += 1;
			}
			if (i > 4 && i < 20)
			{
				pureRed1 += 1;
			}
			if (toCenter > 5 && toCenter < 12)
			{
				bars.Add(drawPos + new Vector2(0, -10 + GetRandomWave(i)), waterEnvLight0, new Vector3(coordX0, 0, 0));
				bars.Add(drawPos + new Vector2(8 * Direction, -10 + GetRandomWave(i + 1)), waterEnvLight1, new Vector3(coordX1, 0, 0));
				bars.Add(drawPos + new Vector2(0, 6), waterEnvLight0, new Vector3(coordX0, 1, 0));

				bars.Add(drawPos + new Vector2(0, 6), waterEnvLight0, new Vector3(coordX0, 1, 0));
				bars.Add(drawPos + new Vector2(8 * Direction, -10 + GetRandomWave(i + 1)), waterEnvLight1, new Vector3(coordX1, 0, 0));
				bars.Add(drawPos + new Vector2(8 * Direction, 6), waterEnvLight1, new Vector3(coordX1, 1, 0));

				bars_highlight.Add(drawPos + new Vector2(0, -10 + GetRandomWave(i)), waterEnvLight0 * pureRed0, new Vector3(coordX0, 0, 0));
				bars_highlight.Add(drawPos + new Vector2(8 * Direction, -10 + GetRandomWave(i + 1)), waterEnvLight1 * pureRed1, new Vector3(coordX1, 0, 0));
				bars_highlight.Add(drawPos + new Vector2(0, 6), waterEnvLight0 * pureRed0, new Vector3(coordX0, 1, 0));

				bars_highlight.Add(drawPos + new Vector2(0, 6), waterEnvLight0 * pureRed0, new Vector3(coordX0, 1, 0));
				bars_highlight.Add(drawPos + new Vector2(8 * Direction, -10 + GetRandomWave(i + 1)), waterEnvLight1 * pureRed1, new Vector3(coordX1, 0, 0));
				bars_highlight.Add(drawPos + new Vector2(8 * Direction, 6), waterEnvLight1 * pureRed1, new Vector3(coordX1, 1, 0));
			}
		}
		Ins.Batch.Draw(liquidTex, bars, PrimitiveType.TriangleList);
		Ins.Batch.Draw(liquidTex_highlight, bars_highlight, PrimitiveType.TriangleList);

		//if(LiquidAreas.Count > 0)
		//{
		//	var rec = LiquidAreas[0];
		//	bars = new List<Vertex2D>();
		//	bars.Add(rec.TopLeft(), Color.White, new Vector3(0, 0, 0));
		//	bars.Add(rec.TopRight(), Color.White, new Vector3(1, 0, 0));

		//	bars.Add(rec.BottomLeft(), Color.White, new Vector3(0, 1, 0));
		//	bars.Add(rec.BottomRight(), Color.White, new Vector3(1, 1, 0));
		//	Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleStrip);
		//}
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
