namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

/// <summary>
/// ai[0] = distance to parent entity <br/>
/// ai[1] = initial angle <br/>
/// </summary>
[Pipeline(typeof(WCSPipeline))]
public class RedAlgae_Small_Dust_SpinAroundEntity : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Projectile ParentEntity;
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;
	public int Frame;

	public override void Update()
	{
		if (ParentEntity is null)
		{
			Velocity *= 0.99f;
			Position += Velocity;
		}
		else
		{
			ai[1] -= 4f / ai[0];
			Vector2 oldPos = Position;
			Position = ParentEntity.Center + new Vector2(0, ai[0]).RotatedBy(ai[1]);
			Velocity = Position - oldPos;
			ai[0] *= 0.99f;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		if (MaxTime - Timer < 60)
		{
			Scale *= 0.95f;
		}
		if (Timer <= 10)
		{
			Scale = MaxScale * Timer / 10f;
		}
		Rotation += 0.05f;
	}

	public override void Draw()
	{
		Rectangle frame = new Rectangle(0, Frame * 12, 12, 12);
		Ins.Batch.Draw(ModAsset.RedAlgae_Small_Dust.Value, Position, frame, Lighting.GetColor(Position.ToTileCoordinates()), Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
