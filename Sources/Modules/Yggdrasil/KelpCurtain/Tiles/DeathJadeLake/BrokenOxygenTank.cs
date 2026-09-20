using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class BrokenOxygenTank : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<AbandonedLakeWreckDust>();
		AddMapEntry(new Color(57, 83, 107));
	}
}
