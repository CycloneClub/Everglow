using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class WaterSluiceBubble : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.frame = new Rectangle(0, 8 * Main.rand.Next(4), 8, 8);
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.scale *= 0.995f;
		dust.velocity.Y += -0.05f;
		dust.velocity *= 0.99f;
		dust.position += dust.velocity;
		if (dust.scale <= 0.15f)
		{
			dust.active = false;
		}
		Tile dustPosTile = TileUtils.SafeGetTile(dust.position.ToTileCoordinates());
		if (dust.position.Y % 16 > dustPosTile.LiquidAmount / 16f)
		{
			dust.active = false;
		}
		return false;
	}
}
