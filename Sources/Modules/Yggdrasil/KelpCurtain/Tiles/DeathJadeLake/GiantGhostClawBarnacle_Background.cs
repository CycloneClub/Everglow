using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

[Pipeline(typeof(WCSPipeline))]
public class GiantGhostClawBarnacle_Background : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void Update()
	{
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.GiantGhostClawBarnacle_Background.Value;
	}

	public override void Draw()
	{
		for (int k = 0; k < 20; k++)
		{
			DrawTentacle(3, Position + new Vector2(100, 100 + k * 8.4f), 150 + 40 * MathF.Sin(k), 3);
		}
		for (int k = 0; k < 10; k++)
		{
			DrawTentacle(2, Position + new Vector2(110, 100 + k * 16.8f), 110 + 40 * MathF.Sin(k), 3);
		}
		for (int k = 0; k < 9; k++)
		{
			DrawTentacle(1, Position + new Vector2(110, 108 + k * 16.8f), 140 + 40 * MathF.Sin(k), 2);
		}
		Lighting.AddLight(Position + new Vector2(180, 122), new Vector3(3f, 0, 0));
		Lighting.AddLight(Position + new Vector2(180, 222), new Vector3(3f, 0, 0));
		var frame = new Rectangle(0, 0, 304, 368);
		int widthSteps = frame.Width / 16; // 304 / 16 = 19.
		int heightSteps = frame.Height / 16; // 368 / 16 = 23;

		var bars = new List<Vertex2D>();
		for (int i = 0; i < widthSteps; i++)
		{
			for (int j = 0; j < heightSteps; j++)
			{
				AddBars(i, j, bars);
				AddBars(i + 1, j, bars);
				AddBars(i, j + 1, bars);

				AddBars(i, j + 1, bars);
				AddBars(i + 1, j, bars);
				AddBars(i + 1, j + 1, bars);
			}
		}
		if (bars.Count > 0)
		{
			Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
		}
		for (int k = 0; k < 5; k++)
		{
			DrawTentacle(1, Position + new Vector2(110, 100 + k * 33.6f), 140 + 40 * MathF.Sin(k), 2);
		}
	}

	public void DrawTentacle(int style, Vector2 position, float length, float width)
	{
		Texture2D tentacle = ModAsset.GiantGhostClawBarnacle_Tentacle.Value;
		var frame = new Rectangle(0, style * 10, 144, 10);
		int steps = 20;
		var bars = new List<Vertex2D>();
		for (int i = 0; i < steps; i++)
		{
			float value = i / (float)steps;
			float wave = MathF.Sin((float)Main.time * 0.06f + value * 5 + TileUtils.GetFixedRandomNumber_SingleSeed((int)(position.Y * 244.1268f))) * 6;
			bars.Add(position + new Vector2(length * value, width + wave), Lighting.GetColor(position.ToTileCoordinates()), new Vector3(value, (frame.Y + frame.Height) / (float)tentacle.Height, 0));
			bars.Add(position + new Vector2(length * value, -width + wave), Lighting.GetColor(position.ToTileCoordinates()), new Vector3(value, frame.Y / (float)tentacle.Height, 0));
		}
		if (bars.Count > 0)
		{
			Ins.Batch.Draw(tentacle, bars, PrimitiveType.TriangleStrip);
		}
	}

	public void AddBars(int i, int j, List<Vertex2D> bars)
	{
		var frame = new Rectangle(0, 0, 304, 368);
		bars.Add(Position + new Vector2(i, j) * 16, Color.White, new Vector3(i * 16 / (float)frame.Width, j * 16 / (float)frame.Height, 0));
	}
}
