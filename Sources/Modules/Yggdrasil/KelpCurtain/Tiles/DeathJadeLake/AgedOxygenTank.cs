using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class AgedOxygenTank : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<AbandonedLakeWreckDust>();
		AddMapEntry(new Color(57, 83, 107));
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (frameX % 36 == 0 && frameY == 0)
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
