namespace Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;

public class LanternFlameRingPipeline : Pipeline
{
	public float Duration = 0;
	public NPC OwnerLanternKing;

	public override void Load()
	{
		effect = ModAsset.LanternFlameRing;
	}

	public override void BeginRender()
	{
		if (OwnerLanternKing == null || !OwnerLanternKing.active || OwnerLanternKing.type != ModContent.NPCType<LanternGhostKing>())
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.active)
				{
					if (npc.type == ModContent.NPCType<LanternGhostKing>())
					{
						OwnerLanternKing = npc;
					}
				}
			}
		}
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_RGB.Value);
		if (OwnerLanternKing != null && OwnerLanternKing.active && OwnerLanternKing.type == ModContent.NPCType<LanternGhostKing>())
		{
			LanternGhostKing lanternGhostKing = OwnerLanternKing.ModNPC as LanternGhostKing;
			if (lanternGhostKing != null)
			{
				Duration = lanternGhostKing.RingRadius / 600f - 1;
			}
			else
			{
				Duration *= 0.95f;
			}
		}
		else
		{
			Duration *= 0.95f;
		}
		Duration = Math.Clamp(Duration, 0, 1);
		effect.Parameters["uDuration"].SetValue(1 - Duration);
		Texture2D LanternFlameRingColor = Commons.ModAsset.Trail.Value;
		Ins.Batch.BindTexture<Vertex2D>(LanternFlameRingColor);
		Main.graphics.GraphicsDevice.Textures[1] = ModAsset.HeatMap_Flame_Ring_Lantern.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
		Main.graphics.GraphicsDevice.Textures[2] = Commons.ModAsset.Noise_Cell.Value;
		Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.LinearWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
