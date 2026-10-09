using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowWoodChair : ModTile
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

		DustType = ModContent.DustType<BlueGlow>();
		AdjTiles = new int[] { TileID.Chairs };

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

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
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
			info.TargetDirection = 1; // Facing right if sat down on the right alternate (added through addAlternate in SetStaticDefaults earlier)

		// The anchor represents the bottom-most tile of the chair. This is used to align the entity hitbox
		// Since i and j may be from any coordinate of the chair, we need to adjust the anchor based on that
		info.AnchorTilePosition.X = i; // Our chair is only 1 wide, so nothing special required
		info.AnchorTilePosition.Y = j;

		if (tile.TileFrameY % NextStyleHeight == 0)
			info.AnchorTilePosition.Y++; // Here, since our chair is only 2 tiles high, we can just check if the tile is the top-most one, then move it 1 down
	}

	public override bool RightClick(int i, int j)
	{
		return FurnitureUtils.ChairRightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		FurnitureUtils.ChairMouseOver<Items.Furnitures.GlowWoodChair>(i, j);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
			zero = Vector2.Zero;
		Texture2D tex = ModAsset.GlowWoodChairGlow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(0.8f, 0.8f, 0.8f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);

		base.PostDraw(i, j, spriteBatch);
	}
}
