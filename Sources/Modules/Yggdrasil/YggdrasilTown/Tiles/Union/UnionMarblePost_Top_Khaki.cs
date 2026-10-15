using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionMarblePost_Top_Khaki : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newAlternate.AnchorAlternateTiles = new[] { Type, ModContent.TileType<UnionMarblePost_Bottom_Khaki>() | ModContent.TileType<UnionMarblePost_Body_Khaki>() | ModContent.TileType<UnionMarblePost_Top_Khaki>() };

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = default;

		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();
		AddMapEntry(new Color(226, 202, 181));
	}
}
