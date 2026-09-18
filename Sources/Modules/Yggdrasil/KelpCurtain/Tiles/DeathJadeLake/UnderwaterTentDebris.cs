using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class UnderwaterTentDebris : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			20,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<AbandonedLakeWreckDust>();
		AddMapEntry(new Color(109, 92, 75));
	}
}
