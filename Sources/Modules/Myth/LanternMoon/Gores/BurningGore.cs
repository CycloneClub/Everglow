namespace Everglow.Myth.LanternMoon.Gores;

public abstract class BurningGore : VisualGore
{
	public float RotateSpeed;

	/// <summary>
	/// 轻度值,下降速度的减缓程度,0~0.1为佳
	/// </summary>
	public float LightValue = 0;

	/// <summary>
	/// 溶解动画灰度图
	/// </summary>
	public Texture2D DissolveAnimationTexture;

	/// <summary>
	/// 不溶解的部分贴图
	/// </summary>
	public Texture2D NoDissolvePartTexture;

	/// <summary>
	/// 是否启用骨骼
	/// </summary>
	public bool HasBone = false;

	/// <summary>
	/// 随机值
	/// </summary>
	public float[] ai;

	public virtual void SetRandomValues()
	{
	}

	public override void OnSpawn()
	{
		base.OnSpawn();
		SetRandomValues();
	}

	public override void Update()
	{
		Timer++;
		Scale *= 0.999f;
		if ((Width <= 0 || Height <= 0) && Texture is not null)
		{
			Width = Texture.Width;
			Height = Texture.Height;
			Weight = Width * Height * Main.rand.NextFloat(0.85f, 1.15f);
		}
		if (TileCollide)
		{
			float velocityValue = Velocity.Length() / 25f;
			velocityValue = Math.Clamp(velocityValue, 0.0f, 1.0f);
			if (TileUtils.PlatformCollision(Position + new Vector2(Velocity.X, 0)))
			{
				Velocity.X = 0;
			}
			if (TileUtils.PlatformCollision(Position + new Vector2(0, Velocity.Y)))
			{
				Velocity.Y = 0;
			}
			else
			{
				if (!NoGravity)
				{
					Velocity.Y += LightValue;
					Velocity.X += Main.windSpeedCurrent / Width * 15f;
				}
			}
		}
		else
		{
			if (!NoGravity)
			{
				Velocity.Y += LightValue;
				Velocity.X += Main.windSpeedCurrent / Width * 15f;
			}
		}

		Rotation += RotateSpeed;
		Velocity *= MathF.Pow(0.999f, Velocity.Length() / Weight * 2500);

		Position += Velocity;

		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		if (NoDissolvePartTexture == null)
		{
			return;
		}
		Vector2 v0 = Position + new Vector2(-Width, -Height).RotatedBy(Rotation) * 0.5f * Scale;
		Vector2 v1 = Position + new Vector2(Width, -Height).RotatedBy(Rotation) * 0.5f * Scale;
		Vector2 v2 = Position + new Vector2(-Width, Height).RotatedBy(Rotation) * 0.5f * Scale;
		Vector2 v3 = Position + new Vector2(Width, Height).RotatedBy(Rotation) * 0.5f * Scale;

		Alpha = (MaxTime - Timer) / 120f;
		Alpha = Math.Clamp(Alpha, 0.0f, 1.0f);

		Color c0 = Lighting.GetColor((v0 / 16f).ToPoint()) * Alpha;
		Color c1 = Lighting.GetColor((v1 / 16f).ToPoint()) * Alpha;
		Color c2 = Lighting.GetColor((v2 / 16f).ToPoint()) * Alpha;
		Color c3 = Lighting.GetColor((v3 / 16f).ToPoint()) * Alpha;

		var bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, new Vector3(0, 0, 0)),
			new Vertex2D(v1, c1, new Vector3(1, 0, 0)),

			new Vertex2D(v2, c2, new Vector3(0, 1, 0)),
			new Vertex2D(v3, c3, new Vector3(1, 1, 0)),
		};
		Ins.Batch.Draw(NoDissolvePartTexture, bars, PrimitiveType.TriangleStrip);
	}

	public virtual void DrawDissolvePart()
	{
		Vector2 v0 = Position + new Vector2(-Width, -Height).RotatedBy(Rotation) * 0.5f * Scale;
		Vector2 v1 = Position + new Vector2(Width, -Height).RotatedBy(Rotation) * 0.5f * Scale;
		Vector2 v2 = Position + new Vector2(-Width, Height).RotatedBy(Rotation) * 0.5f * Scale;
		Vector2 v3 = Position + new Vector2(Width, Height).RotatedBy(Rotation) * 0.5f * Scale;

		Alpha = (MaxTime - Timer) / 120f;
		Alpha = Math.Clamp(Alpha, 0.0f, 1.0f);

		Color c0 = Lighting.GetColor((v0 / 16f).ToPoint()) * Alpha;
		Color c1 = Lighting.GetColor((v1 / 16f).ToPoint()) * Alpha;
		Color c2 = Lighting.GetColor((v2 / 16f).ToPoint()) * Alpha;
		Color c3 = Lighting.GetColor((v3 / 16f).ToPoint()) * Alpha;

		float alpha2 = (Timer - 100) / (MaxTime - 100f);
		alpha2 = Math.Clamp(alpha2, 0.0f, 1.0f);

		var bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, new Vector3(0, 0, alpha2)),
			new Vertex2D(v1, c1, new Vector3(1, 0, alpha2)),

			new Vertex2D(v2, c2, new Vector3(0, 1, alpha2)),
			new Vertex2D(v3, c3, new Vector3(1, 1, alpha2)),
		};
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
