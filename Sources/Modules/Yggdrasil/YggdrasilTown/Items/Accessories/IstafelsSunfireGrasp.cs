using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Cooldowns;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.Audio;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class IstafelsSunfireGrasp : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
		MagicDamageBonus * 100f,
		MagicCritBonus,
		IstafelsSunfireGraspPlayer.ElementDebuffBuildUpRate * 100f,
		IstafelsSunfireGrasp_FireBall.BuildUpMax,
		IstafelsSunfireGraspPlayer.FireBallCooldown / 60f,
		IstafelsSunfireGrasp_FireBall.ContactDamage - 1,
		IstafelsSunfireGrasp_FireBall.BuffDuration / 60f,
		Charred.LifeRegenReductionFromDot / 2f,
		Charred.DefenseReduction * 100f,
		IstafelsSunfireGraspPlayer.SkillDistance / 16f,
		IstafelsSunfireGraspPlayer.SkillDuration / 60f,
		IstafelsSunfireGraspPlayer.SkillDamageBonus * 100f,
		IstafelsSunfireGraspPlayer.SkillCooldown / 60f);

	public const float MagicDamageBonus = 0.11f;
	public const float MagicCritBonus = 5f;

	public override void SetStaticDefaults()
	{
		Item.SetNameOverride("Istafel's Sunfire Grasp");
	}

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 38;

		Item.accessory = true;

		Item.rare = ItemRarityID.Expert;
		Item.value = Item.buyPrice(gold: 10);
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "Expert", this.GetLocalization("RarityLabel").Value));
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetDamage<MagicDamageClass>() += MagicDamageBonus;
		player.GetCritChance<MagicDamageClass>() += MagicCritBonus;
		player.GetModPlayer<IstafelsSunfireGraspPlayer>().IstafelsSunfireGraspEnable = true;
		player.GetModPlayer<YggdrasilPlayer>().istafelsSunfireGrasp = true;
	}

	public class IstafelsSunfireGraspPlayer : ModPlayer
	{
		public const float ElementDebuffBuildUpRate = 2f;
		public const int FireBallCooldown = 600;

		public const int SkillCooldown = 3600;
		public const int SkillDuration = 1200;
		public const int SkillDistance = 1200;
		public const float SkillDamageBonus = 0.5f;

		public bool IstafelsSunfireGraspEnable { get; set; }

		public bool SkillEnable { get; set; } = false;

		public IstafelsSunfireGrasp_FireBall FireBallProj { get; private set; }

		public override void ResetEffects()
		{
			if (!IstafelsSunfireGraspEnable)
			{
				if (FireBallProj != null)
				{
					FireBallProj.BuildUp = 0;
				}
			}

			IstafelsSunfireGraspEnable = false;
		}

		public override void PostUpdateMiscEffects()
		{
			// Handle skill trigger
			if (Player.HasCooldown<IstafelsSunfireGraspSkillCooldown>()
				|| !SkillEnable)
			{
				return;
			}

			Skill();
		}

		/// <summary>
		/// The skill of <see cref="IstafelsSunfireGrasp"/>
		/// </summary>
		private void Skill()
		{
			// If the target is currently receiving flame debuffs, all debuffs currently placed on that enemy
			// immediately produce DMG equal to 100% of their original DMG
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.friendly || npc.townNPC || npc.dontTakeDamage || npc.Center.Distance(Player.Center) > SkillDistance)
				{
					continue;
				}

				if (HasFlameDebuff(npc))
				{
					ApplyFlameDebuffDamage(npc);

					// Generate skill VFX
					Projectile.NewProjectile(Player.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IstafelsSunfireGrasp_SkillVFX>(), 0, 0, Player.whoAmI, npc.whoAmI, Math.Max(npc.width, npc.height));
				}
			}

			// Add skill buff and cooldown debuff
			Player.AddCooldown(IstafelsSunfireGraspSkillCooldown.ID, SkillCooldown);
			Player.AddBuff(ModContent.BuffType<IstafelsSunfireGraspSkillBuff>(), SkillDuration);

			// Play sound effect
			SoundEngine.PlaySound(SoundID.Item130, Player.Center);

			// Update symbol
			SkillEnable = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (IstafelsSunfireGraspEnable && hit.DamageType == DamageClass.Magic)
			{
				target.AddElementalDebuffBuildUp(Player, BurnDebuff.ID, (int)(damageDone * ElementDebuffBuildUpRate));

				ManageFireBall(target, damageDone);
			}
		}

		private void ManageFireBall(NPC target, int damageDone)
		{
			if (!Player.HasCooldown<IstafelsSunfireGraspFireBallCooldown>())
			{
				// If don't have fire ball, create one
				if (FireBallProj == null || !FireBallProj.Projectile.active)
				{
					var proj = Projectile.NewProjectileDirect(Player.GetSource_FromAI(), Player.Center, Player.velocity, ModContent.ProjectileType<IstafelsSunfireGrasp_FireBall>(), 1, 1.2f, Player.whoAmI);
					Player.AddBuff(ModContent.BuffType<IstafelsSunfireGraspFireBallBuff>(), 2);
					FireBallProj = (IstafelsSunfireGrasp_FireBall)proj.ModProjectile;
				}

				// Add build-up
				FireBallProj.BuildUp += damageDone / 3;

				// If build-up is max, shoot fire ball
				if (FireBallProj.BuildUp > IstafelsSunfireGrasp_FireBall.BuildUpMax)
				{
					Player.AddCooldown(IstafelsSunfireGraspFireBallCooldown.ID, FireBallCooldown);

					FireBallProj.Active(target);
					FireBallProj = null;
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Player.HasBuff<IstafelsSunfireGraspSkillBuff>())
			{
				if (HasFlameDebuff(target))
				{
					modifiers.FinalDamage.Additive += SkillDamageBonus;
				}
			}
		}

		private static bool HasFlameDebuff(NPC target)
		{
			return BuffUtils.VanillaFlameDebuffs.Any(target.HasBuff)
				|| target.HasBuff(ModContent.BuffType<Charred>());
		}

		private static void ApplyFlameDebuffDamage(NPC target)
		{
			var vanillaDotDamage = target.GetVanillaDotDamage(BuffUtils.VanillaFlameDebuffs);

			var charredIndex = target.FindBuffIndex(ModContent.BuffType<Charred>());
			var charredDamage = charredIndex >= 0 ? target.buffTime[charredIndex] * Charred.LifeRegenReductionFromDot : 0;

			var totalDamage = vanillaDotDamage + charredDamage;

			target.lifeRegenCount -= totalDamage;
			if (target.life < 1)
			{
				target.life = 1;
			}
		}
	}
}
