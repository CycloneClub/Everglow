using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee
{
	public class ResistanceWireBayonet_proj_stab : StabbingProjectile_Stab
	{
		public float Power = 0;

		public override void SetCustomDefaults()
		{
			StabColor = new Color(71, 89, 99);
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
			if (Power > 10)
			{
				target.AddBuff(BuffID.OnFire, (int)Power * 10);
			}

			bool hasHit = target.HasBuff(ModContent.BuffType<HasHitByResistenceWireBayonet>());
			float mulDamage = 1f;
			mulDamage += Power * 0.6f / 100f;
			if (!hasHit)
			{
				target.AddBuff(ModContent.BuffType<HasHitByResistenceWireBayonet>(), 360000000);
				mulDamage *= 2.2f;
			}
			modifiers.FinalDamage += Math.Max(0, 180 - distanceValue) / 180f;
			modifiers.FinalDamage *= mulDamage;

			// ResistanceWireBayonet rWB = null;
			// if (player.HeldItem.ModItem is not null)
			// {
			// rWB = player.HeldItem.ModItem as ResistanceWireBayonet;
			// }
			// if (rWB != null)
			// {
			// if (rWB.Power > 10)
			// {
			// rWB.Power -= 10;
			// }
			// else
			// {
			// rWB.Power = 0;
			// }
			// }
			base.ModifyHitNPC(target, ref modifiers);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			ResistanceWireBayonet rWB = null;
			if (player.HeldItem.ModItem is not null)
			{
				rWB = player.HeldItem.ModItem as ResistanceWireBayonet;
			}
			if (rWB != null)
			{
				Power = rWB.Power;
			}
			base.AI();
		}
	}
}
