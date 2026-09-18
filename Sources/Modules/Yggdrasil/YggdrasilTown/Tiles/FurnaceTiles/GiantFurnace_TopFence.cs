using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

[Pipeline(typeof(WCSPipeline))]
public class GiantFurnace_TopFence : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
	public override void Update()
	{
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.GiantFurnace_TopFence.Value;
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		float width = 42;
		float height = 3;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Color lightColor0 = Lighting.GetColor((int)Position.X / 16 + i, (int)Position.Y / 16 + j);
				Color lightColor1 = Lighting.GetColor((int)Position.X / 16 + i + 1, (int)Position.Y / 16 + j);
				Color lightColor2 = Lighting.GetColor((int)Position.X / 16 + i, (int)Position.Y / 16 + j + 1);
				Color lightColor3 = Lighting.GetColor((int)Position.X / 16 + i + 1, (int)Position.Y / 16 + j + 1);

				bars.Add(Position + new Vector2(i, j) * 16, lightColor0, new Vector3(i / width, j / height, 0));
				bars.Add(Position + new Vector2(i + 1, j) * 16, lightColor1, new Vector3((i + 1) / width, j / height, 0));
				bars.Add(Position + new Vector2(i, j + 1) * 16, lightColor2, new Vector3(i / width, (j + 1) / height, 0));

				bars.Add(Position + new Vector2(i, j + 1) * 16, lightColor2, new Vector3(i / width, (j + 1) / height, 0));
				bars.Add(Position + new Vector2(i + 1, j) * 16, lightColor1, new Vector3((i + 1) / width, j / height, 0));
				bars.Add(Position + new Vector2(i + 1, j + 1) * 16, lightColor3, new Vector3((i + 1) / width, (j + 1) / height, 0));
			}
		}

		if (bars.Count <= 0)
		{
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));

			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}
}
