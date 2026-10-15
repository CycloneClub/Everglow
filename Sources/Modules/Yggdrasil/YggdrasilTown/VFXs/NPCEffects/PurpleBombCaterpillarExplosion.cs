namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;

public class PurpleBombCaterpillarExplosionPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.PurpleBombCaterpillarExplosion;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
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

[Pipeline(typeof(PurpleBombCaterpillarExplosionPipeline))]
public class PurpleBombCaterpillarExplosionDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (Timer < 30f)
		{
			Lighting.AddLight(Position, new Vector3(0.5f, 0.2f, 0.7f) * Timer / 30f);
		}
		else
		{
			Lighting.AddLight(Position, new Vector3(0.5f, 0.2f, 0.7f) * Math.Clamp((120 - Timer) / 30f, 0, 5));
		}
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color drawColor = new Color(0.05f, 0.05f, 0.05f, 0f);
		if (Timer > MaxTime - 20)
		{
			drawColor *= (MaxTime - Timer) / 20f;
		}
		float timeValue = Timer * 0.02f;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0.2f, 0.2f, timeValue)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(0.8f, 0.2f,  timeValue)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0.2f, 0.8f, timeValue)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(0.8f, 0.8f,  timeValue)),
		};
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
