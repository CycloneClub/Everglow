using Everglow.Commons.UI;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.UI;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

[Pipeline(typeof(WCSPipeline))]
public class QuestCounter_Consultant : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public bool MouseOverConsultant = false;

	public override void Update()
	{
		MouseOverConsultant = false;
		Rectangle girlHitBox = new Rectangle((int)Position.X - 19, (int)Position.Y - 32, 38, 32);
		if (girlHitBox.Contains(Main.MouseWorld.ToPoint()))
		{
			Main.instance.MouseText("Furnace Points Redemption");
			MouseOverConsultant = true;
			if (Main.mouseRight && Main.mouseRightRelease && CanInteract())
			{
				SpecialShopSystem.Instance.Open<FurnaceScoreShopUI>();
			}
		}
		base.Update();
	}

	public override void Draw()
	{
		SpriteEffects flip = SpriteEffects.None;
		int k = Player.FindClosest(Position, 1, 1);
		int dir = 1;
		if (k >= 0)
		{
			Player closestPlayer = Main.player[k];
			if (closestPlayer.Center.X < Position.X)
			{
				flip = SpriteEffects.FlipHorizontally;
				dir = -1;
			}
		}
		Ins.Batch.Draw(Texture, Position, null, Lighting.GetColor(Position.ToTileCoordinates()), 0, new Vector2(19, 32), 1f, flip);

		if (MouseOverConsultant && CanInteract())
		{
			Texture2D chatBubble = TextureAssets.Chat.Value;
			Ins.Batch.Draw(chatBubble, Position - Main.screenPosition + new Vector2(-16 + 16 * dir, -16), null, Lighting.GetColor(Position.ToTileCoordinates()), 0, new Vector2(0, chatBubble.Height), 1f, flip);
		}
	}

	public bool CanInteract()
	{
		Player player = Main.LocalPlayer;
		return (player.Center - Position).Length() < new Vector2(player.lastTileRangeX, player.lastTileRangeY).Length() * 16 + 16 && player.chest == -1;
	}
}
