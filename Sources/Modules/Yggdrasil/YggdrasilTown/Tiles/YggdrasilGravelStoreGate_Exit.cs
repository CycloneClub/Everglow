using Everglow.SubSpace;
using Everglow.SubSpace.Tiles;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilGravelStoreGate_Exit : RoomDoorTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileUtils.DefaultToMultiTileWall(5, 5);
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(86, 62, 44));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override bool RightClick(int i, int j)
	{
		RoomManager.ExitALevelOfRoom();
		return true;
	}

	public override void MouseOver(int i, int j)
	{
		Main.instance.MouseText("Exit Union.");
	}
}
