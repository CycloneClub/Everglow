using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionOfficeDoorClosed : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.NotReallySolid[Type] = true;
		TileID.Sets.DrawsWalls[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		// TileID.Sets.OpenDoorID[Type] = ModContent.TileType<UnionOfficeDoorOpen>();
		TileID.Sets.RoomNeeds.CountsAsDoor[Type] = true;

		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();

		AdjTiles = new int[] { TileID.ClosedDoor };

		// Names
		AddMapEntry(new Color(200, 200, 200));

		// Placement
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 6;
		TileObjectData.newTile.Origin = new Point16(0, 0);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = false;
		TileObjectData.newTile.StyleWrapLimit = 36;
		TileObjectData.newTile.StyleLineSkip = 1; // When a door closes, each tile randomize between 3 different options. StyleLineSkip ensures that those tiles are interpreted as the correct style.
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(0, 1);
		TileObjectData.addAlternate(0);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(0, 2);
		TileObjectData.addAlternate(0);
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

	public override bool RightClick(int i, int j)
	{
		return base.RightClick(i, j);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		int tileCoordX = i - tile.TileFrameX / 18;
		int tileCootdY = j - tile.TileFrameY / 18;
		Rectangle tileRectangle = new Rectangle(tileCoordX * 16, tileCootdY * 16, 32, 96);
		Rectangle playerMoveBox = player.Hitbox;

		// Test Code
		// for (int t = 0; t < 20; t++)
		// {
		// Dust dust = Dust.NewDustDirect(new Vector2(tileCoordX * 16, tileCootdY * 16), 32, 96, DustID.ShimmerSpark);
		// dust.velocity *= 0;
		// dust.noGravity = true;
		// }
		playerMoveBox.X += (int)(player.velocity.X * 3) - 1;
		playerMoveBox.Y += (int)(player.velocity.Y * 3);
		playerMoveBox.Width += 2;
		if (tileRectangle.Intersects(playerMoveBox))
		{
			int direction = 1;
			if (player.Center.X > i * 16 + 8)
			{
				direction = -1;
			}
			if (CanOpenDoor(i, j, direction))
			{
				OpenOfficeDoor(i, j, direction);
			}
			else if (CanOpenDoor(i, j, -direction))
			{
				OpenOfficeDoor(i, j, -direction);
			}
		}
	}

	public bool CanOpenDoor(int i, int j, int direction)
	{
		Tile tile = Main.tile[i, j];
		int originalTileFrameX = tile.TileFrameX;
		int originalTileFrameY = tile.TileFrameY;
		if (direction == 1)
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - originalTileFrameX / 18 + x + 2, j - originalTileFrameY / 18 + y);
					if (checkTile.HasTile)
					{
						if (!Main.tileCut[checkTile.TileType])
						{
							return false;
						}
					}
				}
			}
		}

		if (direction == -1)
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - originalTileFrameX / 18 + x - 3, j - originalTileFrameY / 18 + y);
					if (checkTile.HasTile)
					{
						if (!Main.tileCut[checkTile.TileType])
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	public void OpenOfficeDoor(int i, int j, int direction)
	{
		Tile tile = Main.tile[i, j];
		int originalTileFrameX = tile.TileFrameX;
		int originalTileFrameY = tile.TileFrameY;
		var firstTile = TileUtils.SafeGetTile(i - originalTileFrameX / 18, j - originalTileFrameY / 18);
		if (firstTile.TileType == Type)
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - originalTileFrameX / 18 + x, j - originalTileFrameY / 18 + y);
					if (checkTile.TileType == Type)
					{
						checkTile.HasTile = false;
					}
				}
			}
		}
		if (direction == 1)
		{
			for (int x = 0; x < 4; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - originalTileFrameX / 18 + x + 1, j - originalTileFrameY / 18 + y);
					checkTile.TileType = (ushort)ModContent.TileType<UnionOfficeDoorOpen>();
					checkTile.TileFrameX = (short)(x * 18 + 72);
					checkTile.TileFrameY = (short)(y * 18);
					checkTile.HasTile = true;
				}
			}
		}

		if (direction == -1)
		{
			for (int x = 0; x < 4; x++)
			{
				for (int y = 0; y < 6; y++)
				{
					Tile checkTile = TileUtils.SafeGetTile(i - originalTileFrameX / 18 + x - 3, j - originalTileFrameY / 18 + y);
					checkTile.TileType = (ushort)ModContent.TileType<UnionOfficeDoorOpen>();
					checkTile.TileFrameX = (short)(x * 18);
					checkTile.TileFrameY = (short)(y * 18);
					checkTile.HasTile = true;
				}
			}
		}
	}
}
