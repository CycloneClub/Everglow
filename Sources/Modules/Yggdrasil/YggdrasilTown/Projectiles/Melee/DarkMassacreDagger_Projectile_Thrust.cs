using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class DarkMassacreDagger_Projectile_Thrust : StabbingProjectile_Stab
{
	public override void SetCustomDefaults()
	{
		StabColor = new Color(209, 55, 7);
		StabShade = 0.2f;
		StabDistance = 0.70f;
		StabEffectWidth = 0.4f;
	}

	public override void DrawEffect(Color lightColor)
	{
		base.DrawEffect(lightColor);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		Player player = Main.player[Projectile.owner];
		float distanceValue = (player.Center - target.Center).Length();
		modifiers.FinalDamage += Math.Max(0, 180 - distanceValue) / 180f;
	}

	public override void AI()
	{
		base.AI();
	}

	public override void HitTile()
	{
		// SoundEngine.PlaySound(SoundID.Dig.WithPitchOffset(Main.rand.NextFloat(0.6f, 1f)), Projectile.Center);
		base.HitTile();
	}
}
