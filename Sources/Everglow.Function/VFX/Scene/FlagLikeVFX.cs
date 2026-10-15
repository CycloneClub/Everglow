using Everglow.Commons.Enums;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.Scene;

/// <summary>
/// Texture must sized in any times of 16, such as 32x48, 16x16, 48x64...
/// </summary>
[Pipeline(typeof(WCSPipeline))]
public class FlagLikeVFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public Vector2[,] Meshgrid;

	public Vector2 AnchorOffset = Vector2.zeroVector;

	public bool Flip_H = false;

	/// <summary>
	/// This associate with the distort magnitude in wind push.
	/// </summary>
	public float Hardness = 1f;

	public void GenerateMeshgrid()
	{
		int width = Texture.Width / 16;
		width += 1;
		int height = Texture.Height / 16;
		height += 1;
		Meshgrid = new Vector2[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Meshgrid[x, y] = new Vector2(x, y) * 16 + OriginTilePos.ToWorldCoordinates() + AnchorOffset;
			}
		}
	}

	public override void Update()
	{
		if (Meshgrid == null || Meshgrid == default)
		{
			GenerateMeshgrid();
		}
		for (int x = 0; x < Meshgrid.GetLength(0); x++)
		{
			for (int y = 0; y < Meshgrid.GetLength(1); y++)
			{
				Vector2 origPos = new Vector2(x, y) * 16 + OriginTilePos.ToWorldCoordinates() + AnchorOffset;
				if (y > 0)
				{
					origPos.X = Meshgrid[x, y - 1].X;
					Meshgrid[x, y].Y += MathF.Pow(Meshgrid.GetLength(1) - y, 2) * 0.006f;
				}
				Meshgrid[x, y] += GetWindPush(Meshgrid[x, y].X, Meshgrid[x, y].Y, Hardness);
				Meshgrid[x, y] += (origPos - Meshgrid[x, y]) * 0.2f;

				if (y < Meshgrid.GetLength(1) - 1)
				{
					PushFrom(ref origPos, ref Meshgrid[x, y + 1]);
				}
				if (y > 0)
				{
					PushFrom(ref origPos, ref Meshgrid[x, y - 1]);
				}
				if (x < Meshgrid.GetLength(0) - 1)
				{
					PushFrom(ref origPos, ref Meshgrid[x + 1, y]);
				}
				if (x > 0)
				{
					PushFrom(ref origPos, ref Meshgrid[x - 1, y]);
				}
			}
		}
		base.Update();
	}

	public void PushFrom(ref Vector2 origPosition, ref Vector2 targetPos)
	{
		Vector2 totarget = targetPos - origPosition;
		Vector2 force = totarget.NormalizeSafe() * (totarget.Length() - 16) * 1f;
		origPosition += force;
		targetPos -= force;
	}

	public override void Draw()
	{
		if (Meshgrid == null || Meshgrid == default)
		{
			GenerateMeshgrid();
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < Meshgrid.GetLength(0) - 1; x++)
		{
			for (int y = 0; y < Meshgrid.GetLength(1) - 1; y++)
			{
				AddVertex(bars, x, y);
				AddVertex(bars, x + 1, y);
				AddVertex(bars, x, y + 1);

				AddVertex(bars, x, y + 1);
				AddVertex(bars, x + 1, y);
				AddVertex(bars, x + 1, y + 1);
			}
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}

	public void AddVertex(List<Vertex2D> bars, int xIndex, int yIndex)
	{
		Vector2 position = Meshgrid[xIndex, yIndex];
		Vector2 texCoord = new Vector2(xIndex, yIndex) / Texture.Size() * 16;
		if (Flip_H)
		{
			texCoord.X = 1 - texCoord.X;
		}
		bars.Add(position, Lighting.GetColor(position.ToTileCoordinates()), new Vector3(texCoord, 0));
	}

	public static Vector2 GetWindPush(Vector2 pos, float hardness)
	{
		return GetWindPush(pos.X, pos.Y, hardness);
	}

	public static Vector2 GetWindPush(float x, float y, float hardness)
	{
		float noise = 0;
		float timeValue = (float)Main.time * 0.08f;

		for (int i = 0; i < 8; i++)
		{
			noise += MathF.Sin((timeValue + x * 0.15f + i) * MathF.Pow(2, i)) * MathF.Pow(2, -i);
		}
		for (int j = 0; j < 8; j++)
		{
			noise += MathF.Sin((timeValue + y * 0.15f) + j * MathF.Pow(2, j)) * MathF.Pow(2, -j);
		}
		float pushX = noise * 1f * Main.windSpeedCurrent + Main.windSpeedCurrent * 4f;
		if (Main.tile[new Vector2(x, y).ToTileCoordinates()].WallType > WallID.None)
		{
			pushX = 0f;
		}
		Vector2 addVel = Vector2.zeroVector;
		foreach (var player in Main.player)
		{
			if (player is not null && player.active)
			{
				if (player.Hitbox.Contains(new Point((int)x, (int)y)))
				{
					addVel += player.velocity * 0.6f;
				}
			}
		}

		return (new Vector2(pushX, 0) + addVel) * hardness;
	}
}
