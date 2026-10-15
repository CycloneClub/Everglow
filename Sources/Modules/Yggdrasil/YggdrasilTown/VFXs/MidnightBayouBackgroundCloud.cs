using Everglow.Yggdrasil.YggdrasilTown.Background;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(BackgroundPipeline))]
public class MidnightBayouBackgroundCloud : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public Texture2D CloudTexture = null;
	public Vector2 Velocity;
	public Vector2 Position;
	public Vector2 AnchorPos;
	public float Fade;
	public float Timer;
	public float MaxTime;
	public float Distance = 6f;
	public float Scale;

	public override void Update()
	{
		// Movement
		Velocity = new Vector2(Main.windSpeedCurrent * 0.6f, 0);
		Position += Velocity;

		// Timer
		Timer++;
		if (Timer >= MaxTime)
		{
			Active = false;
			return;
		}

		// Color
		Fade = 0.4f;
		float timeLeft = MaxTime - Timer;
		float thretholdTime = 120;
		if (timeLeft < thretholdTime)
		{
			Fade *= timeLeft / thretholdTime;
		}
		if (Timer < thretholdTime)
		{
			Fade *= Timer / thretholdTime;
		}
		var background = ModContent.GetInstance<YggdrasilTownBackground>();
		if (background is not null)
		{
			Fade *= background.BackgroundAlphaMidnightBayou;
		}
	}

	public override void Draw()
	{
		if (CloudTexture is null)
		{
			return;
		}
		Vector2 offsetedPos = Position + (Main.screenPosition - AnchorPos) * (1 - 1 / Distance);
		Color drawColor = new Color(1f, 1f, 1f, 0) * Fade;
		Ins.Batch.Draw(CloudTexture, offsetedPos, null, drawColor, 0, CloudTexture.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
