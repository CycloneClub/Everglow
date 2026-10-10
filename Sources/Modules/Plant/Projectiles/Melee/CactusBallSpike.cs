using Everglow.Plant.Buffs;
using Everglow.Plant.Common;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.Plant.Projectiles.Melee;

public class CactusBallSpike : ModProjectile
{
	public override string Texture => "Terraria/Images/Projectile_763";

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(763);
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.usesIDStaticNPCImmunity = true;
		Projectile.idStaticNPCHitCooldown = 10;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<CactusBallBuff>(), 60, false);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(ModContent.BuffType<CactusBallBuff>(), 60, true, false);
	}
}
