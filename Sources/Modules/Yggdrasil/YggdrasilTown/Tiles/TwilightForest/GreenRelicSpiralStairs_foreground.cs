using System.Runtime.InteropServices;
using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

[Pipeline(typeof(WCSPipeline))]
public class GreenRelicSpiralStairs_foreground : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public Point Origin;

	public Rectangle Frame;

	public int SampleDistance = 40;

	/// <summary>
	/// Became 0.05f when local player inside the barnacle.
	/// </summary>
	public float Transparency = 1f;

	public override void Update()
	{
		// bool inside = false;
		// var collideMap = GiantGhostClawBarnacleCollideTile.PixelHideForeground;
		// for (int i = 0; i < collideMap.GetLength(0); i++)
		// {
		// 	for (int j = 0; j < collideMap.GetLength(1); j++)
		// 	{
		// 		if (collideMap[i, j] >= 200)
		// 		{
		// 			Point topLeft = OriginTilePos + new Point(i, j);
		// 			Rectangle collisionFrame = new Rectangle(topLeft.X * 16, topLeft.Y * 16, 16, 16);
		// 			if (Rectangle.Intersect(collisionFrame, Main.LocalPlayer.Hitbox) != Rectangle.emptyRectangle)
		// 			{
		// 				inside = true;
		// 				break;
		// 			}
		// 		}
		// 	}
		// 	if (inside)
		// 	{
		// 		break;
		// 	}
		// }
		// if (inside)
		// {
		// 	Transparency = Transparency * 0.9f + 0.05f * 0.1f;
		// }
		// else
		// {
		// 	Transparency = Transparency * 0.9f + 1f * 0.1f;
		// }
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.GreenRelicSpiralStairs_foreground.Value;
	}

	public override void Draw()
	{
		var frame = Frame;
		int widthSteps = frame.Width / SampleDistance;
		int heightSteps = frame.Height / SampleDistance;

		int vertexCount = widthSteps * heightSteps * 6;
		var bars = new List<Vertex2D>(vertexCount);
		CollectionsMarshal.SetCount(bars, vertexCount);
		Span<Vertex2D> span = CollectionsMarshal.AsSpan(bars);
		int currentSize = 0;
		for (int i = 0; i < widthSteps; i++)
		{
			for (int j = 0; j < heightSteps; j++)
			{
				AddBars(i, j, span, ref currentSize);
				AddBars(i + 1, j, span, ref currentSize);
				AddBars(i, j + 1, span, ref currentSize);

				AddBars(i, j + 1, span, ref currentSize);
				AddBars(i + 1, j, span, ref currentSize);
				AddBars(i + 1, j + 1, span, ref currentSize);
			}
		}
		CollectionsMarshal.SetCount(bars, currentSize);
		if (bars.Count > 0)
		{
			Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
		}
	}

	public void AddBars(int i, int j, Span<Vertex2D> bars, ref int currentSize)
	{
		Vector2 drawPos = Position + new Vector2(i, j) * SampleDistance - Origin.ToVector2() * 16;
		var drawColor = Lighting.GetColor(drawPos.ToTileCoordinates());
		bars[currentSize++] = new Vertex2D(drawPos, drawColor, new Vector3((i * SampleDistance + Frame.X) / (float)Texture.Width, (j * SampleDistance + Frame.Y) / (float)Texture.Height, 0));
	}
}
