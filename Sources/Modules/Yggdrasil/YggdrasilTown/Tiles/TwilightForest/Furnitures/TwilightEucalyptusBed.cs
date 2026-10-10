using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

public class TwilightEucalyptusBed : ModTile
{
	public const int NextStyleHeight = 38; // Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2

	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.CanBeSleptIn[Type] = true; // Facilitates calling ModifySleepingTargetInfo
		TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
		TileID.Sets.IsValidSpawnPoint[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation

		DustType = ModContent.DustType<TwilightEucalyptusWoodDust>(); // You should set a kind of dust manually.
		AdjTiles = new int[] { TileID.Beds };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
		TileObjectData.addTile(Type);

		// Etc
		AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Bed"));
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return true;
	}

	public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
	{
		// Because beds have special smart interaction, this splits up the left and right side into the necessary 2x2 sections
		width = 2; // Default to the Width defined for TileObjectData.newTile
		height = 2; // Default to the Height defined for TileObjectData.newTile
					// extraY = 0; // Depends on how you set up frameHeight and CoordinateHeights and CoordinatePaddingFix.Y
	}

	public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
	{
		// Default values match the regular vanilla bed
		// You might need to mess with the info here if your bed is not a typical 4x2 tile
		info.VisualOffset.Y += 4f; // Move player down a notch because the bed is not as high as a regular bed
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 1;
	}

	public override bool RightClick(int i, int j)
	{
		return FurnitureUtils.BedRightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		FurnitureUtils.BedMouseOver<Items.Placeables.Furniture.TwilightForest.TwilightEucalyptusBed>(i, j);
	}
}
