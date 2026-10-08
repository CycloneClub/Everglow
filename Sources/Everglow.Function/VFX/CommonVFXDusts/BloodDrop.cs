using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class BloodDropPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.BloodDrop;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_bloodDrop.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uIlluminationThreshold"].SetValue(0.99f);
		Texture2D lightness = ModAsset.Point.Value;
		Ins.Batch.BindTexture<Vertex2D>(lightness);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(BloodDropPipeline))]
public class BloodDrop : Visual
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
		Position += Velocity;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Active = false;
			return;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Active = false;
			return;
		}
		Velocity *= 0.98f;
		Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, 0.14f * Scale * 0.1f);
		Scale *= 0.98f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= -0.2f;
			Timer += 10;
		}
		var tile = Main.tile[(int)(Position.X / 16), (int)(Position.Y / 16)];
		if (Position.Y % 1 < tile.LiquidAmount / 256f)
		{
			Timer += 120;
		}
		if (Scale < 0.5f)
		{
			Timer += 20;
		}
	}

	public override void Draw()
	{
		float progress = Timer / MaxTime * 0.6f;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color lightColor = Lighting.GetColor((int)(Position.X / 16f), (int)(Position.Y / 16f));
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + Velocity + toCorner, lightColor, new Vector3(0, 0, progress)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), lightColor, new Vector3(0, 1, progress)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), lightColor, new Vector3(1, 0, progress)),
			new Vertex2D(Position - Velocity * ai[1] + toCorner.RotatedBy(Math.PI * 1), lightColor, new Vector3(1, 1, progress)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
