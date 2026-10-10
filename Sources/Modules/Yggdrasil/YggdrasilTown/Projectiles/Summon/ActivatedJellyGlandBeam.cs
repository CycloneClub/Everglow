using Everglow.Commons.VFX.CommonVFXDusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class ActivatedJellyGlandBeam : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public int TargetWhoAmI => (int)Projectile.ai[0];

	public override string Texture => ModAsset.ActivatedJellyGlandExplosion_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.magic = true;
		Projectile.extraUpdates = 60;
		Projectile.timeLeft = 60;
		Projectile.penetrate = 3;
	}

	public override void OnSpawn(IEntitySource source)
	{
		var lightning = new BranchedLightning(50f, 3f, Projectile.Center, Projectile.velocity.ToRotation(), 24f, (float)(Math.PI / 200));
		Ins.VFXManager.Add(lightning);
	}

	public override void AI()
	{
		if (TargetWhoAmI >= 0)
		{
			NPC target = Main.npc[TargetWhoAmI];
			Vector2 direction = (target.Center - Projectile.Center).NormalizeSafe();
			Projectile.velocity = Projectile.velocity.Length() * direction;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		return false;
	}
}
