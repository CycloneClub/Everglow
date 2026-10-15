using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

[Pipeline(typeof(WCSPipeline))]
public class GiantGhostClawBarnacle_VFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	/// <summary>
	/// Became 0.05f when local player inside the barnacle.
	/// </summary>
	public float Transparency = 1f;

	public override void Update()
	{
		bool inside = false;
		var collideMap = GiantGhostClawBarnacleCollideTile.PixelHideForeground;
		for (int i = 0; i < collideMap.GetLength(0); i++)
		{
			for (int j = 0; j < collideMap.GetLength(1); j++)
			{
				if (collideMap[i, j] >= 200)
				{
					Point topLeft = OriginTilePos + new Point(i, j);
					Rectangle collisionFrame = new Rectangle(topLeft.X * 16, topLeft.Y * 16, 16, 16);
					if (Rectangle.Intersect(collisionFrame, Main.LocalPlayer.Hitbox) != Rectangle.emptyRectangle)
					{
						inside = true;
						break;
					}
				}
			}
			if (inside)
			{
				break;
			}
		}
		if (inside)
		{
			Transparency = Transparency * 0.9f + 0.05f * 0.1f;
		}
		else
		{
			Transparency = Transparency * 0.9f + 1f * 0.1f;
		}
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.GiantGhostClawBarnacle_VFX.Value;
	}

	public override void Draw()
	{
		var frameFront = new Rectangle(0, 0, 304, 368);
		int widthSteps = frameFront.Width / 16; // 304 / 16 = 19.
		int heightSteps = frameFront.Height / 16; // 368 / 16 = 23;

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
		var frameFront = new Rectangle(0, 0, 304, 368);
		var drawColor = Lighting.GetColor(OriginTilePos + new Point(i, j)) * Transparency;
		bars.Add(Position + new Vector2(i, j) * 16, drawColor, new Vector3(i * 16 / (float)frameFront.Width, j * 16 / (float)frameFront.Height, 0));
	}
}
