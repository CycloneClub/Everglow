using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline))]
public class BloodChurch_Scene_FakeLiquid_Dust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public Vector2 Position;

	public Vector2 Velocity;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public float MaxPosY;

	public float Rotation;

	public Rectangle Frame;

	public override void OnSpawn()
	{
	}

	public override void Update()
	{
		Timer++;
		if (Timer >= MaxTime)
		{
			Kill();
			return;
		}
		if (Position.Y > MaxPosY)
		{
			Kill();
			return;
		}
		Position += Velocity;
		Velocity += new Vector2(0, 0.15f);
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.BloodChurch_Scene_FakeLiquid_Dust.Value;
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates()) * 0.5f;
		Ins.Batch.Draw(tex, Position, Frame, drawColor, Rotation, Frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
