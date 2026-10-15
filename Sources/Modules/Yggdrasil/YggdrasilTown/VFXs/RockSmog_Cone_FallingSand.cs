namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class RockSmog_Cone_FallingSandPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.RockSmog_Cone_FallingSand;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_0.Value);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_rockSmog.Value);
		effect.Parameters["uFallingSize"].SetValue(12f);
		effect.Parameters["uFallingDirection"].SetValue(0f);
		Texture2D halo = Commons.ModAsset.Noise_perlin.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(RockSmog_Cone_FallingSandPipeline))]
public class RockSmog_Cone_FallingSandDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public Queue<Vector2> OldPos = new Queue<Vector2>();

	public override void Update()
	{
		if (Velocity.Y < -0.3f)
		{
			OldPos.Enqueue(Position);
		}
		if (OldPos.Count > 60)
		{
			OldPos.Dequeue();
		}
		Position += Velocity;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		Velocity *= 0.97f;
		if (Velocity.Y < -0.3f)
		{
			Velocity.Y += 0.3f;
		}
		if (Scale < 60)
		{
			Scale += 0.4f;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime; // (float)(Main.timeForVisualEffects * 0.008) % 1f;
		float timeValue = (float)(Main.time * 0.002);
		Vector3 lightValue = Lighting.GetColor(Position.ToTileCoordinates()).ToVector3();
		float light = lightValue.Length();
		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2[] OldPoses = OldPos.ToArray();

		if (OldPoses.Length <= 2)
		{
			bars.Add(Position, new Color(0, light, pocession), new Vector3(ai[0], timeValue, light));
			bars.Add(Position, new Color(0, light, pocession), new Vector3(ai[0], timeValue, light));
			bars.Add(Position, new Color(0, light, pocession), new Vector3(ai[0], timeValue, light));
			bars.Add(Position, new Color(0, light, pocession), new Vector3(ai[0], timeValue, light));
		}
		else
		{
			for (int i = 1; i < OldPos.Count; i++)
			{
				Vector2 normal = OldPoses[i] - OldPoses[i - 1];
				normal = Vector2.Normalize(normal).RotatedBy(MathHelper.PiOver2) * Scale;
				float width = 1 - i / (float)(OldPos.Count - 1);
				bars.Add(OldPoses[i] + normal, new Color(0, light, pocession), new Vector3(ai[0] + i / 10f, 0, width));
				bars.Add(OldPoses[i] - normal, new Color(0, light, pocession), new Vector3(ai[0] + i / 10f, 0.8f, width));
			}
		}
		if (bars.Count > 0)
		{
			Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
		}
	}
}
