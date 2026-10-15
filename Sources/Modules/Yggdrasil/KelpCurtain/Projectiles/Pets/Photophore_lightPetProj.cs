using Everglow.Yggdrasil.KelpCurtain.Buffs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Pets
{
	public class Photophore_lightPetProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.LightPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.penetrate = -1;
			Projectile.netImportant = true;
			Projectile.timeLeft *= 5;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.scale = 0.8f;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			// If the player is no longer active (online) - deactivate (remove) the projectile.
			if (!player.active)
			{
				Projectile.active = false;
				return;
			}

			// Keep the projectile disappearing as long as the player isn't dead and has the pet buff.
			if (!player.dead && player.HasBuff(ModContent.BuffType<Photophore_lightPetBuff>()))
			{
				Projectile.timeLeft = 2;
			}
			Projectile.Center = player.MountedCenter + new Vector2(0, -32);

			// Lights up area around it.
			if (!Main.dedServ)
			{
				Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.9f, Projectile.Opacity * 0.1f, Projectile.Opacity * 0.3f);
			}
		}
	}
}
