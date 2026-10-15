using Everglow.Commons.Enums;
using Everglow.Commons.Graphics;
using Everglow.Commons.Interfaces;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.CommonVFX_Rework;

public class SplashPipeline : Pipeline
{
	public override void BeginRender()
	{
	}

	public override void EndRender()
	{
	}

	public override void Load()
	{
		effect = ModAsset.CommonDissolve;
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		foreach (Visual v in visuals)
		{
			var effect = this.effect.Value;
			Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.AnisotropicClamp, RasterizerState.CullNone);
			Main.graphics.graphicsDevice.Textures[1] = ModAsset.Noise_cell.Value;

			effect.Parameters["uvMulti"].SetValue(0.2f);

			effect.Parameters["uTransform"].SetValue(
				Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
				Main.GameViewMatrix.TransformationMatrix *
				Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));

			if (v is Splash s)
			{
				effect.Parameters["uDissolve"].SetValue((float)Math.Pow(1 - s.TimeLeft / s.MaxTimeLeft, 2f));
			}

			effect.CurrentTechnique.Passes[0].Apply();

			v.Draw();

			Ins.Batch.End();
		}
	}
}

[Pipeline(typeof(SplashPipeline), typeof(BloomPipeline))]
public class Splash : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float Gravity = -0.2f;
	public float TimeLeft;
	public float MaxTimeLeft;
	public float Scale = 1;
	public GradientColor Color;
	public float Rotation;
	public Entity Owner;
	public float SpeedLimits = 1;

	public override void OnSpawn()
	{
		Rotation = Main.rand.NextFloat(6.28f);
		texType = Main.rand.Next(3);
	}

	public override void Update()
	{
		Position += Velocity;
		Velocity.Y += Gravity;
		Velocity *= SpeedLimits;
		Scale *= 0.97f;

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

	private int texType = 0;

	public override void Draw()
	{
		Color c = Color.GetColor(1 - TimeLeft / MaxTimeLeft);
		c.A = 0;
		Vector2 drawPos = Position;
		if (Owner != null)
		{
			drawPos += Owner.Center;
		}

		Texture2D tex = texType switch
		{
			0 => ModAsset.Splash_0.Value,
			1 => ModAsset.Splash_1.Value,
			2 => ModAsset.Splash_2.Value,
			_ => ModAsset.Splash_0.Value,
		};

		Ins.Batch.Draw(tex, drawPos, null, c, Rotation, tex.Size() / 2, Scale, 0);
	}
}
