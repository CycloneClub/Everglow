using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class DecaySandSoil : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<DecaySandSoil_Dust>();
		AddMapEntry(new Color(92, 114, 99));
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile bottomTile = TileUtils.SafeGetTile(i, j + 1);
		Tile tile = Main.tile[i, j];
		if (tile.HasTile && tile.TileType == Type)
		{
			if (!bottomTile.HasTile || (!Main.tileSolid[bottomTile.type] && !Main.tileSolidTop[bottomTile.type]))
			{
				int deltaY = 0;
				while (true)
				{
					Tile topTile = TileUtils.SafeGetTile(i, j - deltaY);
					if (topTile.HasTile && topTile.TileType == Type && j - deltaY > 0)
					{
						WorldGen.KillTile(i, j - deltaY, false, false, true);
					}
					else
					{
						break;
					}
					deltaY++;
				}
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		Tile tile = Main.tile[i, j];
		if (tile.HasTile && tile.TileType == Type)
		{
			if (!fail && noItem)
			{
				Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_TileBreak(i, j), new Point(i, j + 1).ToWorldCoordinates(), new Vector2(0, 4), ModContent.ProjectileType<FallingSand_DecaySandSoil>(), 20, 0.5f, owner: -1);
			}
		}
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		if (!fail)
		{
			num = 0;
		}
		base.NumDust(i, j, fail, ref num);
	}
}
