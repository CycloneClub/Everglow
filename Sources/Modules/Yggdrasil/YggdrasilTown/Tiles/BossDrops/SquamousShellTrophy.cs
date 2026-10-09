using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.BossDrops;

// Simple 3x3 tile that can be placed on a wall
public class SquamousShellTrophy : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.FramesOnKillWall[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
		DustType = DustID.WoodFurniture;
	}
}
