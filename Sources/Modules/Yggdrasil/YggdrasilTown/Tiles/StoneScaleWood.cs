using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class StoneScaleWood : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileMerge[Type][ModContent.TileType<CyanVineOreTile>()] = true;
		Main.tileMerge[Type][ModContent.TileType<CyanVineStone>()] = true;
		Main.tileMerge[Type][ModContent.TileType<CyanVineOreSmallUp>()] = true;
		Main.tileMerge[Type][ModContent.TileType<CyanVineOreSmall>()] = true;
		Main.tileMerge[Type][ModContent.TileType<CyanVineOreLargeUp>()] = true;
		Main.tileMerge[Type][ModContent.TileType<CyanVineOreLarge>()] = true;
		Main.tileMerge[Type][ModContent.TileType<CyanVineOreMiddle>()] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkForestSoil>()] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkForestGrass>()] = true;
		Main.tileMerge[Type][ModContent.TileType<TwilightGrassBlock>()] = true;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = true;
		Main.tileShine2[Type] = false;

		DustType = ModContent.DustType<StoneDragonScaleWoodDust>();
		MinPick = 150;
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(77, 66, 63));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
