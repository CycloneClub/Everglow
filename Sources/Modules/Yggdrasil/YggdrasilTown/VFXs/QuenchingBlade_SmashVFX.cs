namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class QuenchingBlade_SmashVFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public int Timer;

	public int MaxTime = 28;

	public int Direction;

	public Vector2 Position;

	public override void Update()
	{
		Timer++;
		if (Timer >= MaxTime)
		{
			Active = false;
			return;
		}
		float timeValue = 1 - Timer / (float)MaxTime;
		Lighting.AddLight(Position, new Vector3(1.4f * MathF.Sqrt(timeValue), timeValue * timeValue * 1f, timeValue * timeValue * timeValue * 2) * 2f);
	}

	public override void Draw()
	{
		float frameCount = 7;
		float timeValue = Timer / (float)MaxTime;
		int frameNumber = (int)(timeValue * frameCount);
		Rectangle frame = new Rectangle(0, frameNumber * 610, 720, 610);
		SpriteEffects effects = Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Ins.Batch.Draw(ModAsset.QuenchingBlade_SmashVFX.Value, Position, frame, Color.White, 0, new Vector2(frame.Width * 0.5f, frame.Height), 1f, effects);
	}
}
