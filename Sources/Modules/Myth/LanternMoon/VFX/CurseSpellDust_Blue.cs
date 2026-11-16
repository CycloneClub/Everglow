using Everglow.Commons.DataStructures;

namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline))]
public class CurseSpellDust_Blue : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 SpinCenter;
	public float SpinSpeed;
	public float Scale;
	public float Timer;
	public float MaxTime;
	public bool Collided = false;

	public float Fade => 1 - Timer / MaxTime;

	public override void Update()
	{
		Timer++;
		Position = (Position - SpinCenter).RotatedBy(SpinSpeed) + SpinCenter;
		if (Timer < 10)
		{
			Scale += 0.13f;
		}
		if (MaxTime - Timer <= 10)
		{
			Scale -= 0.13f;
		}
		Lighting.AddLight(Position, Scale * 0.006f, Scale * 0.006f, Scale * 0.01f);
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		Texture2D tex = Commons.ModAsset.LightPoint2.Value;
		var drawColor = new Color(0.6f, 0.6f, 1f, 0);
		float fade = 1;
		if (MaxTime - Timer < 30)
		{
			fade = (MaxTime - Timer) / 30f;
		}
		Ins.Batch.Draw(tex, Position, null, drawColor, 0, tex.Size() * 0.5f, Scale * fade * 0.25f, SpriteEffects.None);
	}
}
