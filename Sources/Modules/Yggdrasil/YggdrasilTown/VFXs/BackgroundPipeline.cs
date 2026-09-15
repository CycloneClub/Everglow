namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

/// <summary>
/// If some VFX binding with background, use this make sure the matrix is correct.
/// </summary>
public class BackgroundPipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		if (Main.LocalPlayer.gravDir == -1)
		{
			projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, 0, Main.screenHeight, 0, 1);
		}
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			projection);
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
