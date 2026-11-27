namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline))]
public class LanternSwordHitSpark : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public int Style;
	public Vector2 Velocity;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Position += Velocity;
		Velocity *= MathF.Pow(0.98f, Velocity.Length());
		float rotValue = MathF.Sin((float)Main.timeForVisualEffects * 0.03f + ai[0]) * 0.01f;
		Velocity = Velocity.RotatedBy(rotValue);
		if ((MaxTime - Timer) / 100f < Scale)
		{
			Scale = (MaxTime - Timer) / 100f;
		}
	}

	public override void Draw()
	{
		var drawColor = new Color(1f, 1f, 1f, 1f);
		Texture2D spot = ModAsset.LanternSwordDust.Value;
		float rotation = Velocity.ToRotation();
		Rectangle frame = new Rectangle(Style * 7, 0, 7, 7);
		Ins.Batch.Draw(spot, Position, frame, drawColor, rotation, frame.Size() * 0.5f, new Vector2(MathF.Max(Velocity.Length(), 1), 1) * Scale, SpriteEffects.None);
		Lighting.AddLight(Position, new Vector3(1f, 0f, 0.05f) * Scale * 0.1f);
	}
}
