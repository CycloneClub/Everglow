using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class MetroStationSign_PylonSquare : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 5, 0);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		MinPick = int.MaxValue;
		AddMapEntry(new Color(226, 226, 226));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var tile = Main.tile[i, j];
		if (tile.HasTile && tile.TileFrameX == 36 && tile.TileFrameY == 72 && ColliderManager.Instance.OfType<YggdrasilTown_NormalMetro>().Count() <= 0)
		{
			ColliderManager.Instance.Add<YggdrasilTown_NormalMetro>(new Point(i, j).ToWorldCoordinates() + new Vector2(-1400, 0));
		}
	}
}
