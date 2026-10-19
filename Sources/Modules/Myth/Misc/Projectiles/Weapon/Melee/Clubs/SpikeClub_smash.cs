namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class SpikeClub_smash : ClubProjSmash
{
	public override string Texture => ModAsset.SpikeClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.SourceDamage *= 1f + Math.Min(Omega, 0.8f) * 0.5f;
		modifiers.Knockback *= 0.4f;
	}

	public override void Smash(int level = 0)
	{
		if (level == 0)
		{
			for (int x = 0; x < 8; x++)
			{
				Vector2 v = new Vector2(0, Main.rand.NextFloat(4f, 8f)).RotatedByRandom(Math.PI * 2) * 5f;
				Vector2 v2 = new Vector2(0, 124).RotatedBy(Main.rand.NextFloat(-1f, 1f));
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.Center - v * 3 + v2 + new Vector2(0, -74), v, ModContent.ProjectileType<SpikeClubSlash>(), Projectile.damage / 6, 0, Player.whoAmI, Main.rand.NextFloat(-0.05f, 0.05f));
				p0.timeLeft = Main.rand.Next(120, 136);
			}
		}
		if (level == 1)
		{
			for (int x = 0; x < 24; x++)
			{
				Vector2 v = new Vector2(0, Main.rand.NextFloat(6f, 12f)).RotatedByRandom(Math.PI * 2) * 5f;
				Vector2 v2 = new Vector2(0, 204).RotatedBy(Main.rand.NextFloat(-1f, 1f));
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.Center - v * 3 + v2 + new Vector2(0, -134), v, ModContent.ProjectileType<SpikeClubSlash>(), Projectile.damage / 6, 0, Player.whoAmI, Main.rand.NextFloat(-0.05f, 0.05f));
				p0.timeLeft = Main.rand.Next(118, 126) + x * 3;
			}
		}
		base.Smash(level);
	}
}
