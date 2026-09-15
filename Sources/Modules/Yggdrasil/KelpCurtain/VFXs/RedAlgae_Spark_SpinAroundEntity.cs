namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

/// <summary>
/// ai[0] = distance to parent entity <br/>
/// ai[1] = initial angle <br/>
/// </summary>
[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class RedAlgae_Spark_SpinAroundEntity : Visual
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
		Lighting.AddLight(Position, new Vector3(0.5f, 0.45f, 0.4f) * Scale);
	}

	public override void Draw()
	{
		Rectangle frame = new Rectangle(0, 0, 4, 4);
		Ins.Batch.Draw(ModAsset.RedAlgae_Spark.Value, Position, frame, new Color(1f, 1f, 1f, 1f), Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
