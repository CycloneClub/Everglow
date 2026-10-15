using Everglow.Yggdrasil.WorldGeneration;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

[Pipeline(typeof(WCSPipeline))]
public class WaterSluice_Scene_Dust : Visual
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

		Position += Velocity;
		Velocity += new Vector2(0, 0.15f);
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.WaterSluice_Scene_Dust.Value;
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates()) * 0.5f;
		Ins.Batch.Draw(tex, Position, Frame, drawColor * Fade, Rotation, Frame.Size() * 0.5f, Scale, SpriteEffects.None);
	}
}
