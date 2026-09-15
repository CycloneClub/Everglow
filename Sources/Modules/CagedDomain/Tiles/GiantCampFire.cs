using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Commons.VFX.Pipelines;
using Everglow.Commons.VFX.Scene;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class GiantCampFire : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.Origin = new Point16(2, 1);
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(91, 62, 39));
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];

		if (tile.TileFrameX == 36 && tile.TileFrameY == 18)
		{
			GiantCampFire_flame_fore flame = new GiantCampFire_flame_fore { Position = new Vector2(i, j) * 16 + new Vector2(8), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(flame);
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 36 && tile.TileFrameY == 18 && !Main.gamePaused)
		{
			if (Main.rand.NextBool(3))
			{
				var spark = new FireSparkDust
				{
					velocity = new Vector2(0, -8),
					Active = true,
					Visible = true,
					position = new Vector2(i, j) * 16 + new Vector2(8, -8) + new Vector2(Main.rand.NextFloat(-30f, 30f), 0).RotatedByRandom(6.283),
					maxTime = Main.rand.Next(37, 195),
					scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(12.1f, 27.0f)),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.003f, 0.003f) },
				};
				Ins.VFXManager.Add(spark);
			}
			if (Main.rand.NextBool(3))
			{
				var somg = new VaporDust
				{
					velocity = new Vector2(0, -12),
					Active = true,
					Visible = true,
					position = new Vector2(i, j) * 16 + new Vector2(8, -8) + new Vector2(Main.rand.NextFloat(-30f, 30f), 0).RotatedByRandom(6.283),
					maxTime = Main.rand.Next(37, 305),
					scale = Main.rand.NextFloat(40, 100),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(-0.05f, -0.01f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
		}
		base.NearbyEffects(i, j, closer);
	}
}

public class GiantCampFire_flame_fore_Pipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.GiantCampFire_shader;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_GiantCampFire.Value);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_1.Value);
		Texture2D halo = Commons.ModAsset.Noise_flame_1.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(GiantCampFire_flame_fore_Pipeline), typeof(BloomPipeline))]
public class GiantCampFire_flame_fore : TileVFX
{
	public override void OnSpawn()
	{
		Texture = Commons.ModAsset.Noise_flame_0.Value;
	}

	public override void Update()
	{
		Lighting.AddLight(Position, new Vector3(1, 0.7f, 0.2f) * 2);
		base.Update();
	}

	public override void Draw()
	{
		float timeValue = (float)Main.time * 0.014f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = 0; t < 40; t++)
		{
			float value = t / 40f;
			float zValue = value * 1.2f - 0.2f;
			zValue = Math.Max(zValue, 0);
			zValue = MathF.Pow(zValue, 0.4f);
			float colorR = MathF.Sin(MathF.Pow(value, 0.2f) * MathF.PI) * 0.95f;
			float colorG = 0;
			if (t == 0)
			{
				colorG = MathF.Pow(value, 0.4f);
			}
			float xValue = 9 * MathF.Sin(value * 32 + timeValue * 16) * value + value * value * Main.windSpeedCurrent * 870f;
			bars.Add(Position + new Vector2(30 * (1 - value) + xValue, -t * 9f), new Color(colorR, colorG, value, 0f), new Vector3(0, value * 1.3f - timeValue, zValue));
			bars.Add(Position + new Vector2(-30 * (1 - value) + xValue, -t * 9f), new Color(colorR, colorG, value, 0f), new Vector3(1, value * 1.3f - timeValue, zValue));
			if (t == 39)
			{
				bars.Add(Position + new Vector2(xValue, -t * 9f), new Color(0, colorG, 1, 0f), new Vector3(0, value * 1.3f - timeValue, zValue));
				bars.Add(Position + new Vector2(xValue, -t * 9f), new Color(0, colorG, 1, 0f), new Vector3(1, value * 1.3f - timeValue, zValue));
			}
		}
		for (int t = 0; t < 40; t++)
		{
			float value = t / 40f;
			float zValue = value * 1.2f - 0.2f;
			zValue = Math.Max(zValue, 0);
			zValue = MathF.Pow(zValue, 0.4f);
			float colorR = MathF.Sin(MathF.Pow(value, 0.2f) * MathF.PI) * 0.7f;
			float colorG = 0;
			float xValue = 12 * MathF.Sin(value * 32 + timeValue * 16) * value + value * value * Main.windSpeedCurrent * 870f;
			if (t == 0)
			{
				bars.Add(Position + new Vector2(30 * (1 - value) + xValue, -t * 12f), new Color(0, colorG, 1, 0f), new Vector3(0, value * 1.3f - timeValue, zValue));
				bars.Add(Position + new Vector2(-30 * (1 - value) + xValue, -t * 12f), new Color(0, colorG, 1, 0f), new Vector3(1, value * 1.3f - timeValue, zValue));
				colorG = MathF.Pow(value, 0.4f);
			}
			bars.Add(Position + new Vector2(30 * (1 - value) + xValue, -t * 12f), new Color(colorR, colorG, value, 0f), new Vector3(0, value * 1.3f - timeValue, zValue));
			bars.Add(Position + new Vector2(-30 * (1 - value) + xValue, -t * 12f), new Color(colorR, colorG, value, 0f), new Vector3(1, value * 1.3f - timeValue, zValue));
			if (t == 39)
			{
				bars.Add(Position + new Vector2(xValue, -t * 9f), new Color(0, 0, 1, 0f), new Vector3(0, value * 1.3f - timeValue, zValue));
				bars.Add(Position + new Vector2(xValue, -t * 9f), new Color(0, 0, 1, 0f), new Vector3(1, value * 1.3f - timeValue, zValue));
			}
		}
		for (int t = 0; t < 20; t++)
		{
			float value = t / 40f;
			float zValue = value * 1.2f - 0.2f;
			zValue = Math.Max(zValue, 0);
			zValue = MathF.Pow(zValue, 0.4f);
			float colorR = MathF.Sin(MathF.Pow(value, 0.2f) * MathF.PI) * 0.94f;
			float colorG = 0;
			float xValue = 3 * MathF.Sin(value * 32 + timeValue * 16) * value - value * 50;
			if (t == 0)
			{
				bars.Add(Position + new Vector2(12 * (1 - value) + xValue, t * 3.5f - 15), new Color(0, colorG, 1, 0f), new Vector3(0, value * 0.73f + timeValue * 0.75f, zValue));
				bars.Add(Position + new Vector2(-25 * (1 - value) + xValue, t * 3.5f - 15), new Color(0, colorG, 1, 0f), new Vector3(1, value * 0.73f + timeValue * 0.75f, zValue));
				colorG = MathF.Pow(value, 0.4f);
			}
			bars.Add(Position + new Vector2(12 * (1 - value) + xValue, t * 2.5f - 15), new Color(colorR, colorG, value, 0f), new Vector3(0, value * 0.73f + timeValue * 0.75f, zValue));
			bars.Add(Position + new Vector2(-25 * (1 - value) + xValue, t * 2.5f - 15), new Color(colorR, colorG, value, 0f), new Vector3(1, value * 0.73f + timeValue * 0.75f, zValue));
		}
		for (int t = 0; t < 20; t++)
		{
			float value = t / 40f;
			float zValue = value * 1.2f - 0.2f;
			zValue = Math.Max(zValue, 0);
			zValue = MathF.Pow(zValue, 0.4f);
			float colorR = MathF.Sin(MathF.Pow(value, 0.2f) * MathF.PI) * 0.92f;
			float colorG = 0;
			if (t == 0)
			{
				colorG = MathF.Pow(value, 0.4f);
			}
			float xValue = 3 * MathF.Sin(value * 32 + timeValue * 16) * value - value * 10;
			bars.Add(Position + new Vector2(-20 * (1 - value) + xValue, t * 3.5f - 15), new Color(colorR, colorG, value, 0f), new Vector3(0, value * 0.54f + timeValue * 0.95f, zValue));
			bars.Add(Position + new Vector2(22 * (1 - value) + xValue, t * 3.5f - 15), new Color(colorR, colorG, value, 0f), new Vector3(1, value * 0.54f + timeValue * 0.95f, zValue));
		}
		for (int t = 0; t < 20; t++)
		{
			float value = t / 40f;
			float zValue = value * 1.2f - 0.2f;
			zValue = Math.Max(zValue, 0);
			zValue = MathF.Pow(zValue, 0.4f);
			float colorR = MathF.Sin(MathF.Pow(value, 0.2f) * MathF.PI) * 0.94f;
			float colorG = 0;
			if (t == 0)
			{
				colorG = MathF.Pow(value, 0.4f);
			}
			float xValue = 3 * MathF.Sin(value * 32 + timeValue * 16) * value + value * 40;
			bars.Add(Position + new Vector2(-7 * (1 - value) + xValue, t * 2.5f - 15), new Color(colorR, colorG, value, 0f), new Vector3(0, value * 0.63f + timeValue * 1.05f, zValue));
			bars.Add(Position + new Vector2(25 * (1 - value) + xValue, t * 2.5f - 15), new Color(colorR, colorG, value, 0f), new Vector3(1, value * 0.63f + timeValue * 1.05f, zValue));
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
