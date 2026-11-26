using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBallSecretion_Fresh : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		TileID.Sets.NegatesFallDamage[Type] = true;
		TileID.Sets.ChecksForMerge[Type] = true;
		Main.tileMergeDirt[Type] = false;

		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;
		Main.tileMerge[Type][ModContent.TileType<JellyBallNerve>()] = true;
		Main.tileMerge[Type][ModContent.TileType<JellyBallSecretion>()] = true;

		DustType = ModContent.DustType<JellyBallGel_Fresh>();
		HitSound = SoundID.NPCHit1;

		AddMapEntry(new Color(0, 148, 255));
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		WorldGen.TileMergeAttempt(-2, ModContent.TileType<JellyBallSecretion>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
	}
}
