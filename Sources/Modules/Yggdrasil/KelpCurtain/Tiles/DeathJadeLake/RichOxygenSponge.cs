using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class RichOxygenSponge : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<RichOxygenSponge_Dust>();
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(211, 162, 27));
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		Tile tile = Main.tile[i, j];
		tile.LiquidType = LiquidID.Water;
		tile.LiquidAmount = 255;
		Vector2 pos = new Point(i, j).ToWorldCoordinates();
		Projectile p0 = Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_TileBreak(i, j), pos, new Vector2(0, -5), ModContent.ProjectileType<SpongeOxygenBubble>(), 20, 0, owner: -1);
		for (int k = 0; k < 6; k++)
		{
			Dust dust = Dust.NewDustDirect(pos - new Vector2(4) + new Vector2(-8, 0), 16, 16, ModContent.DustType<SpongeOxygenBubble_Small>());
			dust.customData = p0;
			dust.velocity *= 0.3f;
			dust.velocity.Y -= 2f;
		}
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}
}
