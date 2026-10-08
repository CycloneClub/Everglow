using Everglow.Commons.Enums;
using Everglow.Commons.Graphics;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class FlarePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.Flare;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		Ins.Batch.Begin(BlendState.Additive, DepthStencilState.None, SamplerState.AnisotropicClamp, RasterizerState.CullNone);
		Main.graphics.graphicsDevice.Textures[1] = ModAsset.Noise_perlin.Value;
		effect.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(FlarePipeline))]
public class Flare : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float Gravity = -0.2f;
	public float TimeLeft;
	public float MaxTimeLeft;
	public float Scale;
	public GradientColor Color;
	private float rotation;
	public Entity Owner;

	public float SpeedLimits = 1;


	public override void OnSpawn()
	{
		rotation = Main.rand.NextFloat(6.28f);
	}

	public override void Update()
	{
		Position += Velocity;
		Velocity.Y += Gravity;
		Velocity *= SpeedLimits;

		// Scale *= 0.99f;
		TimeLeft--;
		if (TimeLeft <= 0)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position, 10, 10))
		{
			Velocity.Y *= 0.6f;
		}
	}

	public override void Draw()
	{
		Color c = Color.GetColor(1 - TimeLeft / MaxTimeLeft);
		c.A = (byte)((1 - TimeLeft / MaxTimeLeft) * 255);
		Vector2 drawPos = Position;
		if (Owner != null)
		{
			drawPos += Owner.Center;
		}

		Ins.Batch.Draw(ModAsset.Flare_Tex.Value, drawPos, null, c, rotation, new Vector2(64), Scale, 0);
	}
}
