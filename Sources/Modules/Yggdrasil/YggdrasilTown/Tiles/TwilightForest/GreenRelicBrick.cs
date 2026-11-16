using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class GreenRelicBrick : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkForestSoil>()] = true;
		Main.tileMerge[Type][ModContent.TileType<StoneScaleWood>()] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<GreenRelicBrick_Dust>();
		HitSound = SoundID.Dig;
		MinPick = 160;
		AddMapEntry(new Color(35, 58, 58));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
