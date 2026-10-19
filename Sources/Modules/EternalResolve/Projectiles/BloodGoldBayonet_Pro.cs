using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Buffs;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Buffs;

namespace Everglow.EternalResolve.Projectiles
{
	public class BloodGoldBayonet_Pro : StabbingProjectile
	{
		public NPC ProjTarget;

		private ulong nextHealTick;

		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = Color.Red;
			MaxDarkAttackUnitCount = 8;
			OldColorFactor = 0.7f;
			CurrentColorFactor = 0.5f;
			ShadeMultiplicative_Modifier = 0.6f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 0.6f;
			LightColorValueMultiplicative_Modifier = 0.1f;
			AttackEffectWidth = 0.4f;
			HitTileSparkColor = new Color(255, 0, 20, 185);
		}

		public override void AI()
		{
			base.AI();

			Player player = Main.player[Projectile.owner];
			if (ProjTarget == null)
			{
				player.ClearBuff(ModContent.BuffType<BloodSwordStrikeBuff>());
			}
			else if (!ProjTarget.active)
			{
				player.ClearBuff(ModContent.BuffType<BloodSwordStrikeBuff>());
			}
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<BloodShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.7f));
				dust.velocity = vel;
			}
		}

		public override void OnKill(int timeLeft)
		{
			Player player = Main.player[Projectile.owner];
			player.ClearBuff(ModContent.BuffType<BloodSwordStrikeBuff>());
			base.OnKill(timeLeft);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.GameUpdateCount >= nextHealTick && target.type != NPCID.TargetDummy)
			{
				int healing = Main.rand.NextBool(25) ? Math.Min(3, (int)(damageDone * 0.3f)) : 0;
				if (target.HasBuff(ModContent.BuffType<BloodDrinking>()))
				{
					healing = Math.Max(healing, 1);
				}
				if (healing > 0)
				{
					nextHealTick = Main.GameUpdateCount + 20;
					Projectile.NewProjectile(Projectile.GetItemSource_OnHit(target, ModContent.ItemType<BloodGoldBayonet>()), target.Center, Vector2.Zero, ProjectileID.VampireHeal, 5, 0, Projectile.owner, Projectile.owner, healing);
				}
			}
			ProjTarget = target;
			Player player = Main.player[Projectile.owner];
			if (target.life <= 0 || damageDone >= target.life)
			{
				player.ClearBuff(ModContent.BuffType<BloodSwordStrikeBuff>());
			}
			else
			{
				player.AddBuff(ModContent.BuffType<BloodSwordStrikeBuff>(), 114514 * 60);
			}
		}
	}
}
