using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public class SharpBarnacleWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = ModContent.DustType<SharpBarnacle_Dust>();
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(94, 75, 10));
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		//Tile tile = Main.tile[i, j];
		//if (j % 2 == 0 && tile.WallFrameY < 180)
		//{
		//	tile.WallFrameY += 180;
		//}
		//if (j % 2 == 1 && tile.WallFrameY >= 180)
		//{
		//	tile.WallFrameY -= 180;
		//}
		base.PostDraw(i, j, spriteBatch);
	}

	public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
	{
		if (randomizeFrame)
		{
			// Here we make the chance of WallFrameNumber 0 very rare, just for visual variety: https://i.imgur.com/9Irak3p.png
			if (frameNumber == 0 && WorldGen.genRand.NextBool(3, 4))
			{
				frameNumber = WorldGen.genRand.Next(1, 3);
			}
		}
		return base.WallFrame(i, j, randomizeFrame, ref style, ref frameNumber);
	}
}
