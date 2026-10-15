using Everglow.Yggdrasil.WorldGeneration;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline))]
public class DarkDragon_Scene_Dust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;

	public Vector2 Velocity;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public float MaxPosY;

	public float Rotation;

	public float Fade;

	public Rectangle Frame;

	public int MoveLogic = 0;

	public override void OnSpawn()
	{
		var tile = TileUtils.SafeGetTile(Position.ToTileCoordinates());
		if (Collision.IsWorldPointSolid(Position) && !Main.tileSolidTop[tile.TileType])
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
		if (Position.Y > MaxPosY)
		{
			Kill();
			return;
		}
		if (MoveLogic == 0)
		{
			Rotation += 0.1f;
			Scale *= 0.995f;
			Velocity *= 0.99f;
			Velocity.Y += Scale * 0.01f;
			Position += Velocity;
		}
		if (MoveLogic == 1)
		{
			Rotation += 0.1f;
			Scale *= 0.98f;
			Velocity *= 0.98f;
			Position += Velocity;
		}
		if (Scale <= 0.05)
		{
			Kill();
			return;
		}
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.DarkDragon_Scene_Dust.Value;
		Color drawColor = Color.White;
		Ins.Batch.Draw(tex, Position, Frame, drawColor * Fade, Rotation, Frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
