using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionAcanthusWallLace : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Origin = new(0, 0);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[] { 20, };

		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;

		TileObjectData.addTile(Type);
		DustType = DustID.Gold;
		AddMapEntry(new Color(224, 136, 47));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		var tileLeft = Main.tile[i - 1, j];
		var tileRight = Main.tile[i + 1, j];
		var tile = Main.tile[i, j];
		if (tileLeft.TileType == Type && tileRight.TileType == Type)
		{
			tile.TileFrameX = 18;
		}
		if (tileLeft.TileType == Type && tileRight.TileType != Type)
		{
			tile.TileFrameX = 36;
		}
		if (tileLeft.TileType != Type && tileRight.TileType == Type)
		{
			tile.TileFrameX = 0;
		}
		if (tileLeft.TileType != Type && tileRight.TileType != Type)
		{
			tile.TileFrameX = 54;
		}
		return false;
	}
}
