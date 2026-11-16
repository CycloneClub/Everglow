using Everglow.Commons.Enums;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.MEAC.VFX;

[Pipeline(typeof(WCSPipeline))]
public class TrueMeleeHitSlash : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;

	public float Rotation;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public bool SelfLuminous;

	public Color SlashColor;

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
		Texture2D slash_black = ModAsset.StarSlash_Black.Value;
		Texture2D slash = ModAsset.StarSlash.Value;
		Color drawColor = SlashColor;
		if (!SelfLuminous)
		{
			Color lightC = Lighting.GetColor(Position.ToTileCoordinates());
			drawColor.R = (byte)(lightC.R * drawColor.R / 255f);
			drawColor.G = (byte)(lightC.G * drawColor.G / 255f);
			drawColor.B = (byte)(lightC.B * drawColor.B / 255f);
		}
		float value = 1 - Timer / MaxTime;
		drawColor *= value * 1.4f;
		float scaleX = MathF.Pow(value, 3);
		Ins.Batch.Draw(slash_black, Position, null, Color.White * 0.25f, Rotation, slash_black.Size() * 0.5f, new Vector2(scaleX, 1) * Scale, SpriteEffects.None);
		Ins.Batch.Draw(slash, Position, null, drawColor, Rotation, slash.Size() * 0.5f, new Vector2(scaleX, 1) * Scale, SpriteEffects.None);
	}
}
