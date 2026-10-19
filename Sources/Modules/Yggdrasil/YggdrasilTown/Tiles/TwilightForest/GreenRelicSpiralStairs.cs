using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

[Pipeline(typeof(WCSPipeline))]
public class GreenRelicSpiralStairs : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public override void Update()
	{
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.GreenRelicSpiralStairs.Value;
	}

	public override void Draw()
	{
		var frame = new Rectangle(0, 0, 720, 1200);
		int widthSteps = frame.Width / 16;
		int heightSteps = frame.Height / 16;

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
	}

	public void AddBars(int i, int j, List<Vertex2D> bars)
	{
		var frame = new Rectangle(0, 0, 720, 1200);
		var drawColor = Lighting.GetColor(Position.ToTileCoordinates() + new Point(i, j));
		bars.Add(Position + new Vector2(i, j) * 16, drawColor, new Vector3(i * 16 / (float)frame.Width, j * 16 / (float)frame.Height, 0));
	}
}
