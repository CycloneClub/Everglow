using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline))]
public class WaterSluice_Scene_shockwave : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public float Fade;

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
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.WaterSluice_Scene_ShockWave.Value;
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates()) * 0.5f;
		float duration = Timer / MaxTime;
		Frame.X = 0;
		if (duration > 0.25f)
		{
			Frame.X = 8;
		}
		if (duration > 0.5f)
		{
			Frame.X = 16;
		}
		if (duration > 0.75f)
		{
			Frame.X = 24;
		}
		Ins.Batch.Draw(tex, Position, Frame, drawColor * Fade, 0, Frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
