using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class FloorScaleForPlayers : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileSolid[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileBlendAll[Type] = true;
		MinPick = int.MaxValue;
		DustType = ModContent.DustType<Heatproof_Furniture_Dust>();

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 5;

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(3, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(14, 14, 14));
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Player player = Main.LocalPlayer;
		if (player.Bottom.ToTileCoordinates().X == i && player.Bottom.ToTileCoordinates().Y == j)
		{
			if (YggdrasilTownFurnaceSystem.PlayerDropInFloorScaleTimer == 0)
			{
				if (YggdrasilTownFurnaceSystem.CurrentPlayer == null || YggdrasilTownFurnaceSystem.CurrentPlayer != player)
				{
					YggdrasilTownFurnaceSystem.PlayerDropInFloorScaleTimer = 30;
					YggdrasilTownFurnaceSystem.CurrentPlayer = player;
				}
			}
		}
		if (TileUtils.SafeGetTile(player.Bottom.ToTileCoordinates()).TileType != Type)
		{
			if (YggdrasilTownFurnaceSystem.CurrentPlayer is not null && YggdrasilTownFurnaceSystem.CurrentPlayer == player)
			{
				YggdrasilTownFurnaceSystem.PlayerLeaveFloorScaleTimer = 30;
				YggdrasilTownFurnaceSystem.CurrentPlayer = null;
				YggdrasilTownFurnaceSystem.CurrentScore = 0;
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 90 == 72)
		{
			var zero = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			for (int k = 0; k < 8; k++)
			{
				Texture2D tex = Commons.ModAsset.White.Value;
				var frame = new Rectangle(0, 0, 2, 2);
				Color drawColor = new Color(1f, 0.6f, 0.4f, 1f);
				if (YggdrasilTownFurnaceSystem.CurrentPlayer is null)
				{
					drawColor *= MathF.Sin(Main.GlobalTimeWrappedHourly * 12 + k);
				}
				spriteBatch.Draw(tex, new Vector2(i, j) * 16 + new Vector2(11 - k * 10, 3) + zero - Main.screenPosition, frame, drawColor, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
				drawColor.A = 0;
				float bloomSize = 3f;
				if (YggdrasilTownFurnaceSystem.PlayerDropInFloorScaleTimer > 0)
				{
					bloomSize = 3f + YggdrasilTownFurnaceSystem.PlayerDropInFloorScaleTimer / 10f;
				}
				spriteBatch.Draw(tex, new Vector2(i, j) * 16 + new Vector2(11 - k * 10, 3) + zero - Main.screenPosition, frame, drawColor * 0.25f, MathHelper.PiOver4, frame.Size() * 0.5f, bloomSize, SpriteEffects.None, 0);
				if (bloomSize > 3.2f)
				{
					spriteBatch.Draw(tex, new Vector2(i, j) * 16 + new Vector2(11 - k * 10, 3) + zero - Main.screenPosition, frame, drawColor * 0.25f, MathHelper.PiOver4, frame.Size() * 0.5f, bloomSize - 3.2f, SpriteEffects.None, 0);
				}
			}
		}
	}
}
