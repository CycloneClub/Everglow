using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.GeyserAirBuds;

public class GeyserAirBuds_PlacePreview : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileCut[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileNoAttach[Type] = true;
		RegisterItemDrop(ModContent.ItemType<GeyserAirBudsItem>(), 1);

		TileObjectData.newTile.Width = 6;
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 22];
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(2, 3);
		TileObjectData.newTile.AnchorBottom = new(Terraria.Enums.AnchorType.SolidTile, 2, 0);
		TileObjectData.addTile(Type);
		AnimationFrameHeight = 96;

		AddMapEntry(new Color(110, 239, 48));
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		return false;
	}
}
