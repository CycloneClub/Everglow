using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilPlayerRoomDoor_Lock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Origin = new(0, 0);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[4];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(2, 3);

		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(191, 157, 128));
	}

	public override bool RightClick(int i, int j)
	{
		bool canOpen = false;
		foreach (var item in Main.LocalPlayer.inventory)
		{
			if (item.type == ModContent.ItemType<YggdrasilPlayerRoomDoorKey>())
			{
				canOpen = true;
				if (item.stack > 1)
				{
					item.stack--;
				}
				else
				{
					item.TurnToAir();
				}
				break;
			}
		}
		if (canOpen)
		{
			var tile = Main.tile[i, j];
			int topLeftTileX = i - tile.TileFrameX / 18;
			int topLeftTileY = j - tile.TileFrameY / 18;
			for (int x = 0; x < 5; x++)
			{
				for (int y = 0; y < 4; y++)
				{
					var checkTile = Main.tile[topLeftTileX + x, topLeftTileY + y];
					checkTile.TileType = (ushort)ModContent.TileType<YggdrasilPlayerRoomDoor>();
					checkTile.HasTile = true;
					checkTile.TileFrameX = (short)(x * 18);
					checkTile.TileFrameY = (short)(y * 18);
				}
			}
		}
		return base.RightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		Main.instance.MouseText("[i:" + ModContent.ItemType<YggdrasilPlayerRoomDoorKey>() + "]");
	}
}
