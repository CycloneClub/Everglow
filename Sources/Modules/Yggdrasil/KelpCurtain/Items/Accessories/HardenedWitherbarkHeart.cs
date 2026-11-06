using Everglow.Yggdrasil.KelpCurtain.Buffs;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class HardenedWitherbarkHeart : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const int CooldownDuration = 10 * 60; // 10 seconds in frames

	public const float DamageTakenIncrease = 0.5f; // 50% increase in damage taken when player has Fracture debuff.
	public const float DamageTakenReduction = 0.9f; // 90% reduction in damage taken when player does not have Fracture debuff.

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.defense = 10;

		Item.accessory = true;

		Item.value = Item.buyPrice(gold: 2);
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// -50% movement speed
		player.moveSpeed -= 0.5f;
		player.GetModPlayer<HardenedWitherbarkHeartPlayer>().Enable = true;
	}

	public class HardenedWitherbarkHeartPlayer : ModPlayer
	{
		public bool Enable { get; set; } = false;

		public override void ResetEffects()
		{
			Enable = false;
		}

		public override void UpdateEquips()
		{
			if (!Player.HasBuff<FractureDebuff>() && Enable)
			{
				Player.AddBuff(ModContent.BuffType<HardenedSkinBuff>(), 2);
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (Enable)
			{
				if (Player.HasBuff<FractureDebuff>())
				{
					modifiers.FinalDamage.Additive += 0.5f; // Increase damage taken by 50% when player has Fracture debuff.
				}
				else
				{
					Player.AddBuff(ModContent.BuffType<FractureDebuff>(), CooldownDuration);
					modifiers.FinalDamage *= 1f - DamageTakenReduction; // Decrease damage taken by 90% when player does not have Fracture debuff.
				}
			}
		}
	}
}
