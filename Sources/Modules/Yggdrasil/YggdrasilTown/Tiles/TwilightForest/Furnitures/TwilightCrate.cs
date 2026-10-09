using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

public class TwilightCrate : ModTile
{
	public override void SetStaticDefaults()
	{
		DustType = ModContent.DustType<TwilightEucalyptusWoodDust>(); // You should set a kind of dust manually.

		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileTable[Type] = true;

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
		TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item
		TileObjectData.addTile(Type);

		// Etc
		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(200, 200, 200), name);
	}

	public override void NumDust(int x, int y, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}
}
