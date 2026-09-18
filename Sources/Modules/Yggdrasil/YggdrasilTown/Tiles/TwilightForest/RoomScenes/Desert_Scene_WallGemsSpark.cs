using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline))]
public class Desert_Scene_WallGemsSpark : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public float Fade;

	public Color Color;

	public Rectangle Frame;

	public override void OnSpawn()
	{
		if (Collision.IsWorldPointSolid(Position))
		{
			Kill();
			return;
		}
	}

	public override void Update()
	{
		Timer++;
		if (Timer >= MaxTime)
		{
			Kill();
			return;
		}
		float duration = Timer / MaxTime;
		Lighting.AddLight(Position, Color.ToVector3() * (1 - duration) * 0.3f);
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.Desert_Scene_WallGemsSpark.Value;
		Color drawColor = Color.Lerp(Color, Color.White, 0.3f);
		drawColor.A = 0;
		float duration = Timer / MaxTime;
		Frame.X = 0;
		if (duration > 0.5f)
		{
			Frame.X = 14;
		}
		Ins.Batch.Draw(tex, Position, Frame, drawColor * Fade, 0, Frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
