namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

[Pipeline(typeof(WCSPipeline))]
public class GiantFurnace_LavaWindowPipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		effect.Value.CurrentTechnique.Passes[0].Apply();
		effect.Value.Parameters["uSize"].SetValue(0.6f);
		effect.Value.Parameters["uTime"].SetValue((float)Main.time * 0.01f);
		effect.Value.Parameters["uHeatmap"].SetValue(ModAsset.LavaWindow_Color.Value);
		effect.Value.Parameters["uLight"].SetValue(Commons.ModAsset.Noise_smoothIce.Value);
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = ModAsset.GiantFurnace_LavaWindow_shader;
	}
}
