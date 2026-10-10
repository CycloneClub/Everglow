using Terraria.DataStructures;

namespace Everglow.Food.Buffs;

public class FoodBuffGlobalPojectile : GlobalProjectile
{
	public override bool InstancePerEntity => true;

	public int CaramelPuddingBounce = 0;

	public override void OnSpawn(Projectile projectile, IEntitySource source)
	{
		Player player = Main.player[projectile.owner];
		if (player != null && player.active && !player.dead)
		{
			if (player.GetModPlayer<FoodBuffModPlayer>().BlueHawaiiBuff)
			{
				if (projectile.owner == player.whoAmI)
				{
					projectile.velocity *= 1.67f;
				}
			}
			if (player.GetModPlayer<FoodBuffModPlayer>().CantaloupeJellyBuff)
			{
				if (projectile.owner == player.whoAmI && projectile.penetrate >= 0)
				{
					projectile.penetrate++;
				}
			}
		}
	}

	public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
	{
		Player player = Main.player[projectile.owner];
		if (player != null && player.active && !player.dead)
		{
			if (player.GetModPlayer<FoodBuffModPlayer>().CaramelPuddingBuff && CaramelPuddingBounce < 1 && projectile.tileCollide)
			{
				if (projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
				{
					projectile.velocity.X = oldVelocity.X * -0.9f;
				}

				if (projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
				{
					projectile.velocity.Y = oldVelocity.Y * -0.9f;
				}

				CaramelPuddingBounce++;
				return false;
			}
		}
		return base.OnTileCollide(projectile, oldVelocity);
	}

	public override void AI(Projectile projectile)
	{
		Player player = Main.player[projectile.owner];
		if (player != null && player.active && !player.dead)
		{
			if (player.GetModPlayer<FoodBuffModPlayer>().DreamYearningBuff)
			{
				if (projectile.owner == player.whoAmI)
				{
					if (projectile.friendly && projectile.penetrate >= 0)
					{
						Vector2 destination = projectile.Center;
						bool locatedTarget = false;
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							if (Main.npc[i].CanBeChasedBy(projectile, false) && projectile.WithinRange(Main.npc[i].Center, 300) && (projectile.tileCollide || Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1)))
							{
								destination = Main.npc[i].Center;
								locatedTarget = true;
								break;
							}
						}
						if (locatedTarget)
						{
							Vector2 homeDirection = Vector2.Normalize(destination - projectile.Center);
							projectile.velocity = Vector2.Normalize(projectile.velocity + homeDirection * 2.5f) * projectile.velocity.Length();
						}
					}
					else
					{
						base.AI(projectile);
					}
				}
			}
		}
		base.AI(projectile);
	}
}
