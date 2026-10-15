using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.Visuals;

public abstract class DissolveGore : VisualGore
{
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

		List<Vertex2D> bars = new List<Vertex2D>()
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

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, new Vector3(0, 0, alpha2)),
			new Vertex2D(v1, c1, new Vector3(1, 0, alpha2)),

			new Vertex2D(v2, c2, new Vector3(0, 1, alpha2)),
			new Vertex2D(v3, c3, new Vector3(1, 1, alpha2)),
		};
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
