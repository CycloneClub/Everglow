using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class JadeLakeSponge : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			30,
		};
		TileObjectData.newTile.CoordinateWidth = 30;
		TileObjectData.newTile.DrawYOffset = -12;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<JadeLakeSpongeDust>();
		AddMapEntry(new Color(129, 120, 97));
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (frameX % 30 == 0 && frameY == 0)
		{
			Vector2 pos = new Point(i, j).ToWorldCoordinates() + new Vector2(8, 16);
			var p0 = Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_TileBreak(i, j), pos, new Vector2(0, -5), ModContent.ProjectileType<SpongeOxygenBubble>(), 20, 0);
			for (int k = 0; k < 6; k++)
			{
				var dust = Dust.NewDustDirect(pos - new Vector2(4) + new Vector2(-8, 0), 16, 16, ModContent.DustType<SpongeOxygenBubble_Small>());
				dust.customData = p0;
				dust.velocity *= 0.3f;
				dust.velocity.Y -= 2f;
			}
		}
		base.KillMultiTile(i, j, frameX, frameY);
	}
}
