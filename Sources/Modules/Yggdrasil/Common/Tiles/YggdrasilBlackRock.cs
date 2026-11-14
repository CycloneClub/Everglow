using Everglow.Yggdrasil.Common.Dusts;

namespace Everglow.Yggdrasil.Common.Tiles;

public class YggdrasilBlackRock : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileMerge[Type][(ushort)ModContent.TileType<YggdrasilTown.Tiles.StoneScaleWood>()] = true;
		Main.tileMerge[(ushort)ModContent.TileType<YggdrasilTown.Tiles.StoneScaleWood>()][Type] = true;

		Main.tileMerge[(ushort)ModContent.TileType<YggdrasilTown.Tiles.LampWood.DarkForestSoil>()][Type] = true;

		Main.tileMerge[Type][(ushort)ModContent.TileType<YggdrasilTown.Tiles.LampWood.DarkForestGrass>()] = true;
		Main.tileMerge[(ushort)ModContent.TileType<YggdrasilTown.Tiles.LampWood.DarkForestGrass>()][Type] = true;

		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;

		TileID.Sets.ChecksForMerge[Type] = true;

		DustType = ModContent.DustType<YggdrasilBlackRock_Dust>();
		MinPick = 300;
		HitSound = SoundID.Dig;
		AddMapEntry(new Color(18, 18, 40));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		// Keep the host's transition frames while vanilla handles slopes, half blocks and variants.
		WorldGen.TileMergeAttempt(-2, ModContent.TileType<YggdrasilTown.Tiles.LampWood.DarkForestSoil>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
	}
}
