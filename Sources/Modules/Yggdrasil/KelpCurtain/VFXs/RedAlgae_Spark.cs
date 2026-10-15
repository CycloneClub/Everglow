namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class RedAlgae_Spark : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

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
		Lighting.AddLight(Position, new Vector3(0.5f, 0.45f, 0.4f) * Scale);
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.RedAlgae_Spark.Value;
		Ins.Batch.Draw(tex, Position, null, Lighting.GetColor(Position.ToTileCoordinates()), Rotation, tex.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
