namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class PeachBlossom : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public float Fade = 1f;
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
			Fade = (MaxTime - Timer) / 60f;
		}
		Velocity *= 0.9f;
		Velocity += new Vector2(Main.windSpeedCurrent * 0.3f, 0.1f);
		Velocity = Velocity.RotatedBy(MathF.Sin((float)Main.time * 0.03f + ai[0]) * 0.05f);
		if (Collision.IsWorldPointSolid(Position))
		{
			Rotation = 0;
			Velocity *= 0;
			if (MaxTime - Timer > 60)
			{
				Timer = MaxTime - 60;
			}
		}
		else if (TileUtils.SafeGetTile(Position.ToTileCoordinates()).LiquidAmount > 0)
		{
			Rotation = 0;
			Velocity *= 0;
			Position.Y = Position.ToTileCoordinates().ToWorldCoordinates().Y + TileUtils.SafeGetTile(Position.ToTileCoordinates()).LiquidAmount / 255f;
			if (MaxTime - Timer > 60)
			{
				Timer = MaxTime - 60;
			}
		}
		else
		{
			Position += Velocity;
		}
		if (Timer % 5 == 0 && Velocity.Length() > 0.1f)
		{
			Frame++;
			if (Frame >= 10)
			{
				Frame = 0;
			}
		}
		Rotation = MathF.Sin(ai[0]) * 0.5f;
	}

	public override void Draw()
	{
		Rectangle frame = new Rectangle(0, Frame * 12, 12, 12);
		Ins.Batch.Draw(ModAsset.PeachBlossom.Value, Position, frame, Lighting.GetColor(Position.ToTileCoordinates()) * Fade, Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
