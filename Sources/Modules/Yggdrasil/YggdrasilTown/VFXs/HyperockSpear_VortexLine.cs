using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class HyperockSpear_VortexLinePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.HyperockSpear_VortexLine;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_0.Value);
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D FlameColor = ModAsset.HeatMap_HyperockSpear_VortexLine.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(HyperockSpear_VortexLinePipeline))]
public class HyperockSpear_VortexLine : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Queue<Vector2> OldPos = new Queue<Vector2>();
	public Projectile VFXOwner;
	public Vector2 PositiontoProjectile;
	public Vector2 Velocity;
	public bool OnTile;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	private Vector2 vFXOwnerCenterOld;

	public override void Update()
	{
		if (VFXOwner == null || !VFXOwner.active || VFXOwner.type != ModContent.ProjectileType<HyperockSpearProj>())
		{
			Active = false;
			return;
		}
		Vector2 point = vFXOwnerCenterOld + Vector2.One.RotatedBy(VFXOwner.rotation + MathHelper.PiOver2) * 0f;
		HyperockSpearProj HyperockSpearProj = VFXOwner.ModProjectile as HyperockSpearProj;
		if (HyperockSpearProj != null)
		{
			vFXOwnerCenterOld = VFXOwner.Center;

			Vector2 pierceAim = -Velocity - PositiontoProjectile;
			if (pierceAim.Length() < 30)
			{
				Timer += 2;
			}

			// if (HyperockSpearProj.Shot)
			// {
			// Timer++;
			// Velocity = Velocity.RotatedBy(PositiontoProjectile.Length() * 0.3f) * 0.95f;
			// }
			// else
			// {

			// }
			float dis = PositiontoProjectile.Length();
			float disValue = HyperockSpearProj.Power / 4500f;
			Velocity = PositiontoProjectile.RotatedBy(MathHelper.PiOver2 + dis * disValue) * (10f / (dis + 0.01f));
		}
		OldPos.Enqueue(PositiontoProjectile);
		if (OldPos.Count > 30)
		{
			OldPos.Dequeue();
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		PositiontoProjectile += Velocity;
		float pocession = 1 - Timer / MaxTime;
		float c = pocession * Scale * 0.1f;
		c /= (PositiontoProjectile.Length() + 1f) * 0.1f;
		Lighting.AddLight(PositiontoProjectile + point, c * 0.5f, c * 0.1f, c * 0.8f);
	}

	public override void Draw()
	{
		int len = OldPos.Count;
		var bars = new List<Vertex2D>();
		Vector2 point = vFXOwnerCenterOld + Vector2.One.RotatedBy(VFXOwner.rotation + MathHelper.PiOver2) * 0f;
		if (len <= 2)
		{
			for (int i = 1; i < 3; i++)
			{
				bars.Add(PositiontoProjectile + point, new Color(0.3f + ai[0], 1, 0, 0), new Vector3(0 + ai[0], (i + 30 + len) / 17f, 1));
				bars.Add(PositiontoProjectile + point, new Color(0.3f + ai[0], 1, 0, 0), new Vector3(0.6f + ai[0], (i + 30 + len) / 17f, 1));
			}
		}
		else
		{
			Vector2[] pos = OldPos.Reverse().ToArray();
			for (int i = 1; i < len; i++)
			{
				float pocession = Timer / MaxTime;
				if (Timer - i < 20)
				{
					pocession += (20 - Timer + i) / 20f;
				}
				pocession = Math.Clamp(pocession, 0, 1);
				Vector2 normal = pos[i] - pos[i - 1];
				normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
				float width = Scale * (float)Math.Sin(i / (double)len * Math.PI);
				float z1 = (pos[i] + normal * width).Length() * 0.45f - 4;
				float z2 = (pos[i] - normal * width).Length() * 0.15f - 6;
				bars.Add(pos[i] + point + normal * width, new Color(0.3f + ai[0], pocession, 0, 0), new Vector3(0 + ai[0], (i + 30 + len) / 17f, z1));
				bars.Add(pos[i] + point - normal * width, new Color(0.3f + ai[0], pocession, 0, 0), new Vector3(0.6f + ai[0], (i + 30 + len) / 17f, z2));
			}
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
