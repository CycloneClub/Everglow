using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.Pipelines;

/// <summary>
/// 异格世界坐标系Pipeline，会自动剪去Main.screenPosition，且采样方式为PointWrap
/// </summary>
public class WCSPipeline_PointWrap : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		Ins.Batch.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = VFXManager.DefaultEffect;
	}
}
