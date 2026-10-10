using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DecayingWoodCourt;
using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DecayingWoodCourt;

public class WitherWoodTable : ModTile
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

		DustType = ModContent.DustType<WitherWoodDust>(); // You should set a kind of dust manually.
		AdjTiles = new int[] { TileID.Tables };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
		TileObjectData.addTile(Type);

		TileID.Sets.RoomNeeds.CountsAsTable[Type] = true;

		// Etc
		AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Table"));
	}

	public override void NumDust(int x, int y, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}
}
