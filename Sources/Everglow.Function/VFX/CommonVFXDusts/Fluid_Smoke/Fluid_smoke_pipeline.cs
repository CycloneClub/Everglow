using Everglow.Commons.Interfaces;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class Fluid_smoke : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.CurseFlame_highQuality;
	}

	public override void BeginRender()
	{
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Value.Parameters["uTransform"].SetValue(model * projection);
		effect.Value.Parameters["uNoise"].SetValue(ModAsset.Noise_melting.Value);
		effect.Value.Parameters["uLight"].SetValue(ModAsset.Trail.Value);
		Texture2D FlameColor = ModAsset.Cursed_Color2.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.AnisotropicWrap, RasterizerState.CullNone);
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		if (visuals.Count() > 0)
		{
			base.Render(visuals);
		}
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}