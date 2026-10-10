using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class RedpaperGiantAttackProj : ModProjectile
{
	public float Timer = 0;

	public NPC OwnerNPC;

	public override void SetDefaults()
	{
		Projectile.width = 110;
		Projectile.height = 110;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 45;
		Projectile.penetrate = -1;
	}

	public override void AI()
	{
		Timer++;
		Projectile.velocity *= 0;
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<RedpaperGiant>())
		{
			Projectile.Center = OwnerNPC.Center;
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Main.expertMode)
		{
			target.AddBuff(BuffID.Bleeding, 600);
		}
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}
