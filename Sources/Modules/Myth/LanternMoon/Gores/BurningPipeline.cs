using Everglow.Commons.Interfaces;
using Everglow.Commons.VFX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Everglow.Myth.LanternMoon.Gores;

public class BurningPipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();


		// 设置参数
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.Parameters["rand1"].SetValue(40);
		effect.Value.Parameters["rand2"].SetValue(60);
		effect.Value.Parameters["rand3"].SetValue(1200);
		// effect.Value.Parameters["NoiseTexture"].SetValue(Commons.ModAsset.Noise_burn.Value); // 噪声纹理
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		BeginRender();
		foreach (var visual in visuals)
		{
			BurningGore burningGore = visual as BurningGore;
			if (burningGore != null)
			{
				burningGore.DrawDissolvePart();
			}
		}
		EndRender();
	}

	public override void Load()
	{
		effect = ModAsset.BurningFade;
	}
}
