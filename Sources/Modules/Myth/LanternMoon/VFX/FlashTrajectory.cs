namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline))]
public class FlashTrajectory : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float Rotation;
	public float Distance;
	public float Timer;
	public float MaxTime;

	public float Fade => 1 - Timer / MaxTime;

	public Color DrawColor => Color.Lerp(new Color(0.3f, 0.36f, 0.2f, 0f), new Color(1f, 0.9f, 0.4f, 0), MathF.Sqrt(Fade));

	public override void Update()
	{
		Timer++;
		float colorSize = Fade / 255f * 3;
		Lighting.AddLight(Position, DrawColor.R * colorSize, DrawColor.G * colorSize, DrawColor.B * colorSize);
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		Texture2D tex_dark = Commons.ModAsset.StarSlashGray_Black.Value;
		Ins.Batch.Draw(tex_dark, Position, null, new Color(Fade, Fade, Fade, Fade), Rotation, new Vector2(tex_dark.Width * 0.5f, 0), new Vector2(Fade, Distance / 256f), SpriteEffects.None);
		Texture2D tex = Commons.ModAsset.StarSlashGray.Value;
		Ins.Batch.Draw(tex, Position, null, DrawColor, Rotation, new Vector2(tex.Width * 0.5f, 0), new Vector2(Fade * 0.75f, Distance / 256f), SpriteEffects.None);
	}
}
