using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.Furnace;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class BakeryChair : ModTile
{
	public const int NextStyleHeight = 40; // Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2

	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
		TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

		DustType = DustID.BorealWood; // You should set a kind of dust manually.
		AdjTiles = new int[] { TileID.Chairs };

		// Names
		AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Chair"));

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
		TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

		// The following 3 lines are needed if you decide to add more styles and stack them vertically
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); // Facing right will use the second texture style
		TileObjectData.addTile(Type);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
	}

	public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
	{
		// It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity
		Tile tile = Framing.GetTileSafely(i, j);

		// info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
		// info.visualOffset = Vector2.Zero; // Defaults to (0,0)
		info.TargetDirection = -1;
		if (tile.TileFrameX != 0)
		{
			info.TargetDirection = 1; // Facing right if sat down on the right alternate (added through addAlternate in SetStaticDefaults earlier)
		}

		// The anchor represents the bottom-most tile of the chair. This is used to align the entity hitbox
		// Since i and j may be from any coordinate of the chair, we need to adjust the anchor based on that
		info.AnchorTilePosition.X = i; // Our chair is only 1 wide, so nothing special required
		info.AnchorTilePosition.Y = j;

		if (tile.TileFrameY % NextStyleHeight == 0)
		{
			info.AnchorTilePosition.Y++; // Here, since our chair is only 2 tiles high, we can just check if the tile is the top-most one, then move it 1 down
		}
	}

	public override bool RightClick(int i, int j)
	{
		return FurnitureUtils.ChairRightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		FurnitureUtils.ChairMouseOver<HeatproofChair_item>(i, j);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameY == 0)
		{
			Rectangle frame = new Rectangle(0, 0, 24, 38);
			if (tile.TileFrameX == 18)
			{
				frame.X += 26;
			}
			Texture2D tex = ModAsset.BakeryChair_full.Value;
			Vector2 zero = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.zeroVector;
			}
			spriteBatch.Draw(tex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero + new Vector2(-12, -12), frame, Lighting.GetColor(i, j), 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
		return base.PreDraw(i, j, spriteBatch);
	}
}
