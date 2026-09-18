namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class RedAlgae_Small_Dust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public int Frame;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (MaxTime - Timer < 60)
		{
			Scale *= 0.95f;
		}
		if (ai.Length > 0)
		{
			Velocity *= ai[0];
		}
		else
		{
			Velocity *= 0.99f;
		}
		Position += Velocity;
		Rotation += 0.05f;
	}

	public override void Draw()
	{
		Rectangle frame = new Rectangle(0, Frame * 12, 12, 12);
		Ins.Batch.Draw(ModAsset.RedAlgae_Small_Dust.Value, Position, frame, Lighting.GetColor(Position.ToTileCoordinates()), Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
