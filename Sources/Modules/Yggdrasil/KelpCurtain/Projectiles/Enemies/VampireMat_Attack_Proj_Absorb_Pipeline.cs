using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Attack_Proj_Absorb_Pipeline : Pipeline
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
		effect = ModAsset.VampireMat_Attack_Proj_Absorb_Fade;
	}
}
