using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.JellyBall;

[AutoloadEquip(EquipType.Head)]
public class JellyBallBathingCap : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(3, 1, EffectTriggerChance * 100f, DamageReduction * 100f);

	public const float EffectTriggerChance = 0.1f;
	public const float DamageReduction = 0.35f;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(silver: 43, copper: 50);
	}

	public override void UpdateEquip(Player player)
	{
		// 1. + 3 defense
		// ==============
		player.statDefense += 3;

		// 2. + 1 minion slot
		// ==================
		player.maxMinions += 1;

		// 3. Has chance to decrease projectile damage and reflect projectile
		// ==================================================================
		player.GetModPlayer<JellyBallBathingCapPlayer>().JellyBallBathingCapEnable = true;
	}
}

public class JellyBallBathingCapPlayer : ModPlayer
{
	public bool JellyBallBathingCapEnable { get; set; }

	public override void ResetEffects()
	{
		JellyBallBathingCapEnable = false;
	}

	public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
	{
		if (JellyBallBathingCapEnable)
		{
			// 3. Has 10% chance to reduce 35% damage and reflect projectile
			if (Main.rand.NextFloat() < JellyBallBathingCap.EffectTriggerChance)
			{
				// 3.1 Reduce 35% final damage
				modifiers.FinalDamage *= 1.0f - JellyBallBathingCap.DamageReduction;

				// 3.2 Reflect projectile
				proj.hostile = false;
				proj.friendly = true;
				proj.velocity *= -1f;
				proj.extraUpdates += 1;
				proj.penetrate = 1;
			}
		}
	}
}
