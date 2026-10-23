namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class RecoveryBand : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const int EffectTriggerDamageMin = 12;
	public const int LifeRecovery = 5;
	public const int ManaRecovery = 10;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.width = 28;
		Item.height = 20;

		Item.value = Item.buyPrice(platinum: 0, gold: 1);
		Item.rare = ItemRarityID.Blue;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. Recover a bit life and mana on hurt
		// ======================================
		// On player hurt by anything, if damage is at least 12, then recover 5 life and 10 mana
		player.GetModPlayer<RecoveryBandPlayer>().RecoveryBandEnable = true;
	}
}

public class RecoveryBandPlayer : ModPlayer
{
	public bool RecoveryBandEnable { get; set; }

	public override void ResetEffects()
	{
		RecoveryBandEnable = false;
	}

	public override void OnHurt(Player.HurtInfo info)
	{
		if (RecoveryBandEnable && info.Damage >= RecoveryBand.EffectTriggerDamageMin)
		{
			Player.HealLife(RecoveryBand.LifeRecovery);
			Player.HealMana(RecoveryBand.ManaRecovery);
		}
	}
}
