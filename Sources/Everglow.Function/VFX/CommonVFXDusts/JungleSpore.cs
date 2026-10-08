using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class JungleSporePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.JungleSpore;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_JungleSpore.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D halo = Commons.ModAsset.Point.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(JungleSporePipeline), typeof(BloomPipeline))]
public class JungleSporeDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;


	public override void Update()
	{
		ai[1] *= 0.99f;
		Position += Velocity;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Timer = MaxTime;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
		}
		Velocity *= Math.Max(0, 1 - Scale * 0.01f);
		Velocity += new Vector2(Main.windSpeedCurrent * 0.04f, 0.001f * Scale);
		Scale *= 0.995f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= -0.2f;
			Timer += 5;
		}
		var tile = Main.tile[(int)(Position.X / 16), (int)(Position.Y / 16)];
		if (Position.Y % 1 < tile.LiquidAmount / 256f)
		{
			Timer += 5;
		}
		if (Scale < 0.5f)
		{
			Timer += 20;
		}
		float progress = 1 - Timer / MaxTime;
		float c = progress * Scale * 0.01f;
		Lighting.AddLight(Position, c * 0.1f, c * 0.8f, 0);
	}

	public override void Draw()
	{
		float progress = 1 - Timer / MaxTime;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, ai[0], progress), new Vector3(0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, ai[0], progress), new Vector3(0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, ai[0], progress), new Vector3(0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, ai[0], progress), new Vector3(0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
