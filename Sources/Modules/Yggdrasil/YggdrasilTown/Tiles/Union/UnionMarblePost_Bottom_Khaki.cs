using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionMarblePost_Bottom_Khaki : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newAlternate.AnchorAlternateTiles = new[] { Type, ModContent.TileType<UnionMarblePost_Bottom_Khaki>() | ModContent.TileType<UnionMarblePost_Body_Khaki>() | ModContent.TileType<UnionMarblePost_Top_Khaki>() };
		TileObjectData.newTile.AnchorBottom = new(Terraria.Enums.AnchorType.SolidTile, TileObjectData.newTile.Width, 0);

		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();
		AddMapEntry(new Color(226, 202, 181));
	}
}
