using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.SpellAndSkull.Projectiles.WaterBolt;

public class WaterBoltDropPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.WaterBoltDrop;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_WaterBoltDrop.Value);
		effect.Parameters["uIlluminationThreshold"].SetValue(0.99f);
		Texture2D lightness = Commons.ModAsset.Point_lowContrast.Value;
		Ins.Batch.BindTexture<Vertex2D>(lightness);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.LinearClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(WaterBoltDropPipeline))]
public class WaterBoltDrop : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float rotation;

	public WaterBoltDrop() { }

	public override void Update()
	{
		position += velocity;
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			timer = maxTime;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
		}
		velocity *= 0.98f;
		velocity += new Vector2(Main.windSpeedCurrent * 0.1f, 0.21f * scale * 0.1f);
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(position, 0, 0))
		{
			velocity *= -0.02f;
			if (Ins.VisualQuality.Low)
			{
				timer += 4;
			}
			else
			{
				if (!Main.rand.NextBool(4))
				{
					timer -= 1;
				}
			}
		}
		var tile = Main.tile[(int)(position.X / 16), (int)(position.Y / 16)];
		if (position.Y % 1 < tile.LiquidAmount / 256f)
		{
			timer += 120;
		}
		if (scale < 0.5f)
		{
			timer += 20;
		}
		float pocession = 1 - timer / maxTime;
		float c = pocession * scale * 0.08f;
		Lighting.AddLight(position, 0, c * 0.3f, c * 0.8f);
	}

	public override void Draw()
	{
		float pocession = timer / maxTime * 0.6f;
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		Color lightColor = Color.White;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(position + velocity + toCorner, lightColor, new Vector3(0, 0, pocession)),
			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 0.5), lightColor, new Vector3(0, 1, pocession)),

			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 1.5), lightColor, new Vector3(1, 0, pocession)),
			new Vertex2D(position - velocity * ai[1] + toCorner.RotatedBy(Math.PI * 1), lightColor, new Vector3(1, 1, pocession)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
