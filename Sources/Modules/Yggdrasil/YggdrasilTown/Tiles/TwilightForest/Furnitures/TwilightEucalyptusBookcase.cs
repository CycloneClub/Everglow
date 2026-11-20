using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

public class TwilightEucalyptusBookcase : ModTile
{
	public override void SetStaticDefaults()
	{
		AddMapEntry(new Color(69, 36, 78), ModContent.GetInstance<Items.Placeables.Furniture.TwilightForest.TwilightEucalyptusBookcase>().DisplayName);
		// Properties
		Main.tileTable[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

		DustType = ModContent.DustType<TwilightEucalyptusWoodDust>(); // You should set a kind of dust manually.
		AdjTiles = new int[] { TileID.Bookcases };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
		TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
		TileObjectData.addTile(Type);

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}
}
