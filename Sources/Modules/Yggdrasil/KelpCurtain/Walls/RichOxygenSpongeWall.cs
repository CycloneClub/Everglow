using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public class RichOxygenSpongeWall : ModWall
{
	public override void SetStaticDefaults()
	{
		DustType = ModContent.DustType<RichOxygenSponge_Dust>();
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(94, 75, 10));
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				Tile checkTile = TileUtils.SafeGetTile(x + i, y + j);
				if (!checkTile.HasTile)
				{
					WorldGen.PlaceTile(x + i, y + j, ModContent.TileType<RichOxygenSponge>());
				}
			}
		}
		base.RandomUpdate(i, j, underground);
	}
}
