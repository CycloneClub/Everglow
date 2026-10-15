using Everglow.Commons.Enums;

namespace Everglow.Commons.VFX;

public class NormalPipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		effect.Value.Parameters["uTransform"].SetValue(Main.Transform *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = VFXManager.DefaultEffect;
	}
}

[Pipeline(typeof(NormalPipeline))]
public class MEACVFX : Visual
{
	// MEACmod的VFX移植，方便代码迁移
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Velocity;
	public Vector2 Center;
	public float Rotation = 0;
	public string TexPath = "MEAC/Images/Ball";

	public Texture2D Texture => ModContent.Request<Texture2D>(TexPath).Value;

	public int TimeLeft;
	public int MaxTimeLeft = 50;
	public float Ai0;
	public float Ai1;
	public float Alpha = 1;
	public float Scale = 1;
	public bool IsWarp = false;
	public Color DrawColor = Color.White;
	public bool OrigDraw = true;
	public bool CanBatch = true;
	public int ExtraUpdates = 0;

	public MEACVFX()
	{
	}

	public struct OwnerInfo
	{
		public bool HasOwner = false;
		public Entity Owner;
		public Vector2 Offset;

		public OwnerInfo()
		{
			HasOwner = false;
			Owner = null;
			Offset = Vector2.Zero;
		}
	}

	public OwnerInfo OwnerData;

	public void SetTimeLeft(int t)
	{
		MaxTimeLeft = t;
		TimeLeft = t;
	}

	public static T Create<T>(Vector2 pos, Vector2 velocity, float rotation = 0, float scale = 1, Entity owner = null)
		where T : MEACVFX, new()
	{
		MEACVFX ee = new T();
		ee.SetDefault();
		ee.Velocity = velocity;
		ee.Center = pos;
		ee.Rotation = rotation;
		ee.TimeLeft = ee.MaxTimeLeft;
		if (scale != 1)
		{
			ee.Scale = scale;
		}

		if (owner != null)
		{
			ee.OwnerData.Owner = owner;
			ee.OwnerData.HasOwner = true;
			ee.OwnerData.Offset = pos - owner.Center;
		}
		Ins.VFXManager.Add(ee);
		return ee as T;
	}

	public virtual void SetDefault()
	{
	}

	public virtual void AI()
	{
	}

	public virtual void AIWithOwner(Entity owner)
	{
		Center = owner.Center + OwnerData.Offset;
		OwnerData.Offset += Velocity;
	}

	public override void Update()
	{
		for (int i = 0; i < ExtraUpdates + 1; i++)
		{
			AI();
			TimeLeft--;
			if (TimeLeft <= 0)
			{
				Kill();
				return;
			}
			Center += Velocity;
			if (OwnerData.HasOwner)
			{
				if (!OwnerData.Owner.active)
				{
					OwnerData.HasOwner = false;
				}
				else
				{
					AIWithOwner(OwnerData.Owner);
				}
			}
		}
	}

	public override void Draw()
	{
		Ins.Batch.Draw(Texture, Center - Main.screenPosition, null, DrawColor * Alpha, Rotation, Texture.Size() / 2, Scale, SpriteEffects.None);
	}
}
