using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class DreamStar_Explosion : ModProjectile
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 5;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			Projectile.width = 60;
			Projectile.height = 60;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void OnSpawn(IEntitySource source)
		{
			Projectile.width += (int)(Projectile.ai[0] * 10);
			Projectile.height += (int)(Projectile.ai[0] * 10);
			for (int x = 0; x < 80 * Projectile.ai[0]; x++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<StarShine_purple_withoutPlayer>());
				dust.scale = Main.rand.NextFloat(0.65f, 1.35f) * Projectile.ai[0];
				dust.color.R = (byte)(dust.scale * 100f);
				if (dust.color.R < 25)
				{
					dust.color.R = 25;
				}
				dust.velocity = new Vector2(0, Main.rand.NextFloat(Main.rand.NextFloat(0f, 4f), 4f)).RotatedByRandom(6.283) * Projectile.ai[0];
			}
			for (int x = 0; x < 30 * Projectile.ai[0]; x++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<StarShine_yellow_withoutPlayer>());
				dust.scale = Main.rand.NextFloat(1f, 1.75f) * Projectile.ai[0];
				dust.color.R = (byte)(dust.scale * 100f);
				if (dust.color.R < 25)
				{
					dust.color.R = 25;
				}
				dust.velocity = new Vector2(0, Main.rand.NextFloat(Main.rand.NextFloat(0f, 7f), 7f)).RotatedByRandom(6.283) * Projectile.ai[0];
			}
			SoundEngine.PlaySound(SoundID.Shatter.WithVolumeScale(Projectile.ai[0]), Projectile.Center);
		}

		public override void AI()
		{
			Projectile.velocity *= 0;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return false;
		}
	}
}
