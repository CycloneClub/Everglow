using Everglow.SpellAndSkull.GlobalItems;
using Terraria;

namespace Everglow.SpellAndSkull.Projectiles.WaterBolt;

public class OldWaterBolt : GlobalProjectile
{
	public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.player[projectile.owner].GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1)
		{
			if (projectile.type == ProjectileID.WaterBolt)
			{
				int type = ModContent.ProjectileType<WaterTeleport>();
				Player player = Main.player[projectile.owner];

				player.GetModPlayer<MagicBookPlayer>().WaterBoltHasHit += 1;
				if (player.GetModPlayer<MagicBookPlayer>().WaterBoltHasHit >= 5)
				{
					if (player.ownedProjectileCounts[type] < 6)
					{
						player.GetModPlayer<MagicBookPlayer>().WaterBoltHasHit = 0;
						Projectile.NewProjectileDirect(projectile.GetSource_OnHit(target), player.Center + new Vector2(0, -30 * player.gravDir), new Vector2(0, -18 * player.gravDir), type, 0, 0, projectile.owner, player.ownedProjectileCounts[type] + 1);
					}
				}
			}
		}
	}
}
