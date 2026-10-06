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
	public string texPath = "MEAC/Images/Ball";

	public Texture2D Texture => ModContent.Request<Texture2D>(texPath).Value;

	public int timeleft;
	public int maxTimeleft = 50;
	public float ai0;
	public float ai1;
	public float alpha = 1;
	public float scale = 1;
	public bool isWarp = false;
	public Color drawColor = Color.White;
	public bool origDraw = true;
	public bool canBatch = true;
	public int extraUpdates = 0;

	public MEACVFX()
	{
	}

	public struct OwnerInfo
	{
		public bool hasOwner = false;
		public Entity owner;
		public Vector2 offset;

		public OwnerInfo()
		{
			hasOwner = false;
			owner = null;
			offset = Vector2.Zero;
		}
	}

	public OwnerInfo ownerInfo;

	public void SetTimeleft(int t)
	{
		maxTimeleft = t;
		timeleft = t;
	}

	public static T Create<T>(Vector2 pos, Vector2 velocity, float Rotation = 0, float scale = 1, Entity owner = null)
		where T : MEACVFX, new()
	{
		MEACVFX ee = new T();
		ee.SetDefault();
		ee.Velocity = velocity;
		ee.Center = pos;
		ee.Rotation = Rotation;
		ee.timeleft = ee.maxTimeleft;
		if (scale != 1)
		{
			ee.scale = scale;
		}

		if (owner != null)
		{
			ee.ownerInfo.owner = owner;
			ee.ownerInfo.hasOwner = true;
			ee.ownerInfo.offset = pos - owner.Center;
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
		Center = owner.Center + ownerInfo.offset;
		ownerInfo.offset += Velocity;
	}

	public override void Update()
	{
		for (int i = 0; i < extraUpdates + 1; i++)
		{
			AI();
			timeleft--;
			if (timeleft <= 0)
			{
				Kill();
				return;
			}
			Center += Velocity;
			if (ownerInfo.hasOwner)
			{
				if (!ownerInfo.owner.active)
				{
					ownerInfo.hasOwner = false;
				}
				else
				{
					AIWithOwner(ownerInfo.owner);
				}
			}
		}
	}

	public override void Draw()
	{
		Ins.Batch.Draw(Texture, Center - Main.screenPosition, null, drawColor * alpha, Rotation, Texture.Size() / 2, scale, SpriteEffects.None);
	}
}
