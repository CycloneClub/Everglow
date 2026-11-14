using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class DevilHeartIronOre : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileNoSunLight[Type] = true;
		Main.tileMerge[(ushort)ModContent.TileType<JadeizedBone>()][Type] = true;
		Main.tileBlockLight[Type] = false;

		TileID.Sets.ChecksForMerge[Type] = true;
		DustType = ModContent.DustType<DevilHeartIronDust>();
		MinPick = 110;
		AddMapEntry(new Color(124, 17, 30));
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		// Keep the host's transition frames while vanilla handles slopes, half blocks and variants.
		WorldGen.TileMergeAttempt(-2, ModContent.TileType<JadeizedBone>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
	}
}
