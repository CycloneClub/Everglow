namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class PearShapedNeedle_HitStar : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public float Timer;
	public float MaxTime;
	public float Rotation;
	public Vector2 Position;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		float timeValue = 1 - Timer / MaxTime;
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates()) * 2.5f;
		drawColor.A = 0;
		float scale = 0.25f;
		Texture2D tex = Commons.ModAsset.StarSlash.Value;
		Ins.Batch.Draw(tex, Position, null, drawColor, Rotation, tex.Size() * 0.5f, new Vector2(timeValue, 1f) * scale, SpriteEffects.None);
		Ins.Batch.Draw(tex, Position, null, drawColor, Rotation + MathHelper.PiOver2, tex.Size() * 0.5f, new Vector2(timeValue, 1.5f) * scale, SpriteEffects.None);
	}
}
