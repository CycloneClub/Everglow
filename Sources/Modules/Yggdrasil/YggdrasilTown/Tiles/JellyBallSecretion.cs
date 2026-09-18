using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBallSecretion : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;

		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;

		DustType = ModContent.DustType<JellyBallGel>();
		HitSound = SoundID.NPCHit1;

		AddMapEntry(new Color(25, 39, 160));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(i, j, 0, 0.5f, 0.94f);
		return base.PreDraw(i, j, spriteBatch);
	}

	public override void RandomUpdate(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		Tile tileLeft = Main.tile[i + 1, j];
		Tile tileRight = Main.tile[i - 1, j];
		if (tile.Slope == SlopeType.Solid && tileLeft.TileType == Type && tileRight.TileType == Type && tileLeft.HasTile && tileRight.HasTile)
		{
			if (Main.rand.NextBool(1))
			{
				Tile t2 = Main.tile[i, j + 1];
				if (!t2.HasTile)
				{
					t2.TileType = (ushort)ModContent.TileType<JellyBeadString>();
					t2.HasTile = true;
				}
			}
			if (Main.rand.NextBool(1))
			{
				Tile t2 = Main.tile[i, j - 1];
				if (!t2.HasTile)
				{
					t2.TileType = (ushort)ModContent.TileType<HaloGrass>();
					t2.TileFrameX = (short)(18 * Main.rand.Next(6));
					t2.HasTile = true;
				}
			}
		}
	}
}
