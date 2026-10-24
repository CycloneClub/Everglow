namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;

public class HeatEmblem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const float BuffTriggerRate = 0.2f;
	public const int BuffDuration = 180;

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.accessory = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 1, silver: 50);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. + 4 Life Regeneration
		// ========================
		player.lifeRegen += 4;

		// 2. Gain "On Fire" debuff when get hurt
		// ======================================
		player.GetModPlayer<HeatEmblemPlayer>().HeatEmblemEnable = true;
	}
}

internal class HeatEmblemPlayer : ModPlayer
{
	public bool HeatEmblemEnable = false;

	public override void ResetEffects()
	{
		HeatEmblemEnable = false;
	}

	// 2. Gain "On Fire" debuff when get hurt
	// ======================================
	// When the wearer get hurt,
	// has 20% chance to inflict an "On Fire!" debuff on wearer.
	public override void OnHurt(Player.HurtInfo info)
	{
		if (HeatEmblemEnable)
		{
			if (Main.rand.NextFloat() < HeatEmblem.BuffTriggerRate)
			{
				Player.AddBuff(BuffID.OnFire, HeatEmblem.BuffDuration);
			}
		}
	}
}
