using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.SubSpace;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class MarbleGate_UnionMarblePost : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public override void Update()
	{
		base.Update();
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.MarbleGate_UnionMarblePost.Value;
	}

	public override void Draw()
	{
		List<Vertex2D> bars = new List<Vertex2D>();
		SpriteBatchUtils.AddVertex_Grid(bars, Position + new Vector2(151, 128), null, Texture.Size() * new Vector2(0.5f, 1f), Texture, false, 128);
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
