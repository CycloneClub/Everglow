using Everglow.Commons.Enums;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.Visuals;

public abstract class VisualGore : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float Scale = 1;
	public float Rotation = 0;
	public float Alpha = 1;
	public bool TileCollide = true;
	public bool NoGravity = false;
	public Texture2D Texture;
	public int Width = -1;
	public int Height = -1;
	public int Timer = 0;
	public int MaxTime = 600;
	public float Weight = 1000f;

	/// <summary>
	/// base.OnSpawn();之前必须填入Texture2D
	/// </summary>
	public override void OnSpawn()
	{
		Timer = 0;
	}

	public override void Update()
	{
		Timer++;
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
				Velocity.X *= -0.75f * velocityValue;
			}
			if (TileUtils.PlatformCollision(Position + new Vector2(0, Velocity.Y)))
			{
				Velocity.Y *= -0.75f * velocityValue;
			}
			else
			{
				if (!NoGravity)
				{
					Velocity.Y += 0.5f;
					Velocity.X += Main.windSpeedCurrent / Width * 20f;
				}
			}
		}
		else
		{
			if (!NoGravity)
			{
				Velocity.Y += 0.15f;
				Velocity.X += Main.windSpeedCurrent / Width * 20f;
			}
		}

		Rotation += Velocity.X / 40f;
		Velocity *= MathF.Pow(0.999f, Velocity.Length() / Weight * 2500);

		Position += Velocity;

		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
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

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, new Vector3(0, 0, 0)),
			new Vertex2D(v1, c1, new Vector3(1, 0, 0)),

			new Vertex2D(v2, c2, new Vector3(0, 1, 0)),
			new Vertex2D(v3, c3, new Vector3(1, 1, 0)),
		};
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
