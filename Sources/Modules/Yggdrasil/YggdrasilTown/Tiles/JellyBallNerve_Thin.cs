using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBallNerve_Thin : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		TileID.Sets.NegatesFallDamage[Type] = true;
		TileID.Sets.ChecksForMerge[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileLighted[Type] = true;

		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;
		Main.tileMerge[Type][ModContent.TileType<JellyBallSecretion>()] = true;
		Main.tileMerge[Type][ModContent.TileType<JellyBallSecretion_Fresh>()] = true;

		DustType = ModContent.DustType<JellyBallNerveDust>();
		HitSound = SoundID.NPCHit1;

		AddMapEntry(new Color(162, 53, 146));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.3f;
		g = 0.05f;
		b = 0.4f;
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		WorldGen.TileMergeAttempt(-2, ModContent.TileType<JellyBallSecretion>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
	}
}
