using Everglow.Yggdrasil.Common.Elevator.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;
using Spine;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class BeetleDashTracePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.BeetleDashTrace;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D halo = Commons.ModAsset.Trail_8.Value;
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

[Pipeline(typeof(BeetleDashTracePipeline))]
public class BeetleDashTraceDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public Projectile ProjectileOwner;
	public Queue<Vector2> Trails = new Queue<Vector2>();

	public override void Update()
	{
		if (ProjectileOwner == null || !ProjectileOwner.active || ProjectileOwner.type != ModContent.ProjectileType<DeadBeetleEgg_beetle>())
		{
			Active = false;
			return;
		}
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
		Position = ProjectileOwner.Center;
		Velocity = ProjectileOwner.velocity;
		Trails.Enqueue(Position);
		if (Trails.Count > 10)
		{
			Trails.Dequeue();
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float timeValue = Timer * 0.02f;
		float colorValue = 1;
		if (Timer > MaxTime - 30)
		{
			colorValue = (MaxTime - Timer) / 30f;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		if (Trails.Count >= 3)
		{
			for (int i = 0; i < Trails.Count; i++)
			{
				Vector2 pos = Trails.ToArray()[i];
				Vector2 posNext = Trails.ToArray()[i] + Velocity;
				if (i != Trails.Count - 1)
				{
					posNext = Trails.ToArray()[i + 1];
				}
				float drawWidth = 1 - i / (float)(Trails.Count - 1);
				Color drawColor = Lighting.GetColor(pos.ToTileCoordinates());
				drawColor.A = 0;
				drawColor *= colorValue;
				drawWidth = MathF.Cos(drawWidth * MathHelper.PiOver2);
				Vector2 width = Utils.SafeNormalize(pos - posNext, Vector2.One).RotatedBy(MathHelper.PiOver2) * 30;
				bars.Add(pos + width, drawColor, new Vector3(i * 0.05f + timeValue, 0, drawWidth));
				bars.Add(pos - width, drawColor, new Vector3(i * 0.05f + timeValue, 1, drawWidth));
			}
		}
		else
		{
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
