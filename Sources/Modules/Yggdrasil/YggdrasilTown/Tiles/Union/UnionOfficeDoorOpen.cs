using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionOfficeDoorOpen : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = true;
		Main.tileNoSunLight[Type] = true;
		TileID.Sets.HousingWalls[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.CloseDoorID[Type] = ModContent.TileType<UnionOfficeDoorClosed>();

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();

		AdjTiles = new int[] { TileID.OpenDoor };

		RegisterItemDrop(ModContent.ItemType<UnionOfficeDoor_Item>(), 0);
		TileID.Sets.CloseDoorID[Type] = ModContent.TileType<UnionOfficeDoorClosed>();

		// Names
		AddMapEntry(new Color(200, 200, 200));

		// Placement
		TileObjectData.newTile.Width = 4;
		TileObjectData.newTile.Height = 6;
		TileObjectData.newTile.Origin = new Point16(0, 0);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(0, 1);
		TileObjectData.addAlternate(0);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(0, 2);
		TileObjectData.addAlternate(0);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(1, 0);
		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
		TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.addAlternate(1);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(1, 1);
		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
		TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.addAlternate(1);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(1, 2);
		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
		TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.addAlternate(1);
		TileObjectData.addTile(Type);
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 1;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<UnionOfficeDoor_Item>();
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		int tileCoordX = i - (tile.TileFrameX % 72) / 18;
		int tileCootdY = j - tile.TileFrameY / 18;
		if (tile.TileFrameX >= 72)
		{
			tileCoordX -= 1;
		}
		else
		{
			tileCoordX += 3;
		}
		Rectangle tileRectangle = new Rectangle(tileCoordX * 16 - 16, tileCootdY * 16, 64, 96);
		Rectangle playerMoveBox = player.Hitbox;
		playerMoveBox.X += (int)(player.velocity.X * 3);
		playerMoveBox.Y += (int)(player.velocity.Y * 3);
		if (!tileRectangle.Intersects(playerMoveBox))
		{
			CloseOfficeDoor(i, j);
		}
	}

	public void CloseOfficeDoor(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int direction;
		int originalTileFrameX = tile.TileFrameX;
		int originalTileFrameY = tile.TileFrameY;
		direction = originalTileFrameX >= 72 ? 1 : -1;
		var firstTile = TileUtils.SafeGetTile(i - (originalTileFrameX % 72) / 18, j - originalTileFrameY / 18);
		if (firstTile.TileType == Type)
		{
			for (int x = 0; x < 4; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - (originalTileFrameX % 72) / 18 + x, j - originalTileFrameY / 18 + y);
					if (checkTile.TileType == Type)
					{
						checkTile.HasTile = false;
					}
				}
			}
		}

		// When the door open toward right.
		if (direction == 1)
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - (originalTileFrameX % 72) / 18 + x - 1, j - originalTileFrameY / 18 + y);
					checkTile.TileType = (ushort)ModContent.TileType<UnionOfficeDoorClosed>();
					checkTile.TileFrameX = (short)(x * 18);
					checkTile.TileFrameY = (short)(y * 18);
					checkTile.HasTile = true;
				}
			}
		}

		if (direction == -1)
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - (originalTileFrameX % 72) / 18 + x + 3, j - originalTileFrameY / 18 + y);
					checkTile.TileType = (ushort)ModContent.TileType<UnionOfficeDoorClosed>();
					checkTile.TileFrameX = (short)(x * 18);
					checkTile.TileFrameY = (short)(y * 18);
					checkTile.HasTile = true;
				}
			}
		}
	}
}
