using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood.Furniture;

public class LampWoodTable : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileTable[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

		DustType = ModContent.DustType<LampWood_Dust>();
		AdjTiles = new int[] { TileID.Tables };

		// Placement?
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
		TileObjectData.addTile(Type);

		TileID.Sets.RoomNeeds.CountsAsTable[Type] = true;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(191, 142, 111), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}
}
