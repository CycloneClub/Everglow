namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class RedAlgaeHitStar : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public float Timer;
	public float MaxTime;
	public float Scale;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		float value = Timer / MaxTime;
		Vector3 color = Vector3.Lerp(new Vector3(1f, 0.9f, 0.8f), new Vector3(0.7f, 0.1f, 0.2f), MathF.Pow(value, 0.5f));
		Lighting.AddLight(Position, color * (1 - value) * Scale * 3);
	}

	public override void Draw()
	{
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float value = Timer / MaxTime;
		Color color = new Color(1f, 0.9f, 0.8f, 0);
		if (value > 0.5f)
		{
			color = new Color(0.7f, 0.1f, 0.2f, 0);
		}
		Ins.Batch.Draw(star, Position, null, color * 0.5f, 0, star.Size() * 0.5f, new Vector2(1f, 1 - value) * Scale, SpriteEffects.None);
		Ins.Batch.Draw(star, Position, null, color * 0.5f, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(1f, 1 - value) * Scale, SpriteEffects.None);
		Ins.Batch.Draw(star, Position, null, color * 0.25f, MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(1f, 1 - value) * Scale * 0.5f, SpriteEffects.None);
		Ins.Batch.Draw(star, Position, null, color * 0.25f, -MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(1f, 1 - value) * Scale * 0.5f, SpriteEffects.None);
	}
}
