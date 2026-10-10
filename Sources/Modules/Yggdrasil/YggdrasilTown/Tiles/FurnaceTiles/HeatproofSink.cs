using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class HeatproofSink : ModTile
{
	public override void SetStaticDefaults()
	{
		TileID.Sets.CountsAsWaterForCrafting[Type] = true;

		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
		TileObjectData.addTile(Type);

		DustType = ModContent.DustType<Heatproof_Furniture_Dust>(); // You should set a kind of dust manually.
		AdjTiles = new int[] { Type };

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}
}
