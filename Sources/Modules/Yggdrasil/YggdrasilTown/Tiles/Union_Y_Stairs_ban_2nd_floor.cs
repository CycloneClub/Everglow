using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline))]
public class Union_Y_Stairs_ban_2nd_floor : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public override void OnSpawn()
	{
		Texture = ModAsset.Union_Y_Stairs_ban_2nd_floor.Value;
	}

	public override void Update()
	{
		Rectangle rectangle = new Rectangle((int)(Position.X - Texture.Width / 2f), (int)(Position.Y - Texture.Height / 2f), Texture.Width, Texture.Height);
		foreach (var player in Main.player)
		{
			// TODO: Enough score in mission system to enter 2nd floor
			if (player.Hitbox.Intersects(rectangle))
			{
				player.Top = new Vector2(player.Top.X, rectangle.Bottom().Y);
				player.velocity.Y = 0;
			}
		}
		base.Update();
	}

	public override void Draw()
	{
		Texture2D texBlack = ModAsset.Union_Y_Stairs_ban_2nd_floor_black.Value;
		Ins.Batch.Draw(texBlack, Position, null, new Color(1f, 1f, 1f, 1f), 0, texBlack.Size() * 0.5f, 1f, SpriteEffects.None);
		Ins.Batch.Draw(Texture, Position, null, new Color(1f, 1f, 1f, 0), 0, Texture.Size() * 0.5f, 1f, SpriteEffects.None);
	}
}
