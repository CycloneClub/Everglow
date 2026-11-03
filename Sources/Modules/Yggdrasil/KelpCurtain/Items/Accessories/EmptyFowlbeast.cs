namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class EmptyFowlbeast : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 44;

		Item.accessory = true;

		Item.value = Item.buyPrice(gold: 10);
		Item.rare = ItemRarityID.LightRed;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// +1 max minion
		player.maxMinions += 1;
		player.GetModPlayer<EmptyFowlbeastPlayer>().Enable = true; // Increase damage taken by 20%.
	}

	public class EmptyFowlbeastPlayer : ModPlayer
	{
		public bool Enable { get; set; } = false;

		public override void ResetEffects()
		{
			Enable = false;
		}

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (Enable)
			{
				modifiers.FinalDamage.Additive += 0.2f;
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Enable && proj.minion)
			{
				// Apply additional magic damage equal to 7% of player max mana to the target when hit by a minion projectile.
				int damageAmount = (int)(0.07f * Player.statManaMax2);
				Player.ApplyDamageToNPC(target, damageAmount, 0, 0, damageType: DamageClass.Magic);
			}
		}
	}
}
