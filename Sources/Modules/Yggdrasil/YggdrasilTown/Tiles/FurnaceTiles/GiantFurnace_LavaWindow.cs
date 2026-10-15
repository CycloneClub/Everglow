using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

[Pipeline(typeof(GiantFurnace_LavaWindowPipeline), typeof(BloomPipeline))]
public class GiantFurnace_LavaWindow : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void Update()
	{
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.GiantFurnace_LavaWindow.Value;
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		Color drawColor = new Color(1f, 1f, 1f, 1f);
		var pos = Position - Main.screenPosition;
		bars.Add(pos + new Vector2(0, 0), drawColor, new Vector3(0, 0, 0));
		bars.Add(pos + new Vector2(Texture.Width, 0), drawColor, new Vector3(1, 0, 0));
		bars.Add(pos + new Vector2(0, Texture.Height), drawColor, new Vector3(0, 1, 0));
		bars.Add(pos + new Vector2(Texture.Width, Texture.Height), drawColor, new Vector3(1, 1, 0));

		if (bars.Count <= 0)
		{
			bars.Add(pos, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(pos, Color.Transparent, new Vector3(0, 0, 0));

			bars.Add(pos, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(pos, Color.Transparent, new Vector3(0, 0, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
