using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class DrainOutlet : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.Width = 4;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<WaterErodedGreenBrickDust>();
		AddMapEntry(new Color(75, 76, 86));
		MinPick = int.MaxValue;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		int style = -1;
		if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
		{
			style = 0;
		}
		if (tile.TileFrameX == 36 && tile.TileFrameY == 18)
		{
			style = 1;
		}
		if (tile.TileFrameX == 18 && tile.TileFrameY == 36)
		{
			style = 2;
		}
		if (tile.TileFrameX == 36 && tile.TileFrameY == 36)
		{
			style = 3;
		}
		Point topLeft = new Point(i, j) - new Point(tile.TileFrameX / 18, tile.TileFrameY / 18);
		if (style >= 0 && tile.LiquidType == LiquidID.Water && tile.LiquidAmount == 255)
		{
			int timeValue0 = (int)((Main.time * 0.1f + topLeft.X + topLeft.Y) % 40);
			if (timeValue0 == style * 10)
			{
				Vector2 pos = new Point(i, j).ToWorldCoordinates();
				Projectile p0 = Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_TileBreak(i, j), pos, new Vector2(0, -5), ModContent.ProjectileType<SpongeOxygenBubble>(), 20, 0, owner: -1);
				for (int k = 0; k < 6; k++)
				{
					Dust dust = Dust.NewDustDirect(pos - new Vector2(4) + new Vector2(-8, 0), 16, 16, ModContent.DustType<SpongeOxygenBubble_Small>());
					dust.customData = p0;
					dust.velocity *= 0.3f;
					dust.velocity.Y -= 2f;
				}
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}
}
