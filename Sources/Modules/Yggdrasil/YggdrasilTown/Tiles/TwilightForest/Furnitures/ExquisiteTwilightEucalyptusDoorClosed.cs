using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

public class ExquisiteTwilightEucalyptusDoorClosed : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.NotReallySolid[Type] = true;
		TileID.Sets.DrawsWalls[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.OpenDoorID[Type] = ModContent.TileType<ExquisiteTwilightEucalyptusDoor>();

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

		DustType = ModContent.DustType<TwilightEucalyptusWoodDust>();
		AdjTiles = new int[] { TileID.ClosedDoor };

		AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

		TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.ClosedDoor, 0));
		TileObjectData.addTile(Type);
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 1;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
	}
}
