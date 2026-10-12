using Everglow.Myth.Acytaea.NPCs;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSword_following : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 3600000;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
	}

	public Vector2 EndPos = Vector2.Zero;
	public NPC Owner = new NPC();

	public override void OnSpawn(IEntitySource source)
	{
		int index = (int)Projectile.ai[0];
		if (index >= 0 && index < 200)
		{
			Owner = Main.npc[index];
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override void AI()
	{
		Projectile.tileCollide = false;
		if (Owner == null || !Owner.active)
		{
			Projectile.Kill();
		}
		if (Owner.type == ModContent.NPCType<Acytaea_Boss>())
		{
			Projectile.direction = Owner.direction;
			Projectile.rotation = MathHelper.PiOver4 * 3 + Projectile.velocity.X * 0.04f;
			Vector2 aimCenter = Owner.Center + new Vector2(-30 * Owner.direction, 20 + 10 * MathF.Sin((float)Main.time * 0.002f));
			Vector2 toAim = aimCenter - Projectile.Center - Projectile.velocity;
			if (toAim.Length() > 5)
			{
				toAim = Utils.SafeNormalize(toAim, Vector2.zeroVector) * 27;
			}

			Projectile.velocity = Projectile.velocity * 0.95f + toAim * 0.05f;
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return true;
	}

	public override bool ShouldUpdatePosition()
	{
		return true;
	}
}
