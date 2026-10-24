namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class FragmentsOfWreckage : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const float LifeLossForBonusMax = 120f;
	public const float DamageBonus = 0.06f;
	public const float MeleeSpeedBonus = 0.06f;
	public const float PickSpeedBonus = 0.1f;
	public const int DefenseBonus = 8;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.width = 34;
		Item.height = 30;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(platinum: 0, gold: 1);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. Slightly increases stats based on life loss
		// ==============================================
		var lifeLoss = player.statLifeMax2 - player.statLife;

		// A maximum of 120 life loss will be counted
		var bonusScaler = lifeLoss / LifeLossForBonusMax;
		if (bonusScaler > 1)
		{
			bonusScaler = 1;
		}
		else if (bonusScaler < 0)
		{
			bonusScaler = 0;
		}

		// +6% damage
		player.GetDamage(DamageClass.Generic) += DamageBonus * bonusScaler;

		// +6% melee speed
		player.meleeSpeed += MeleeSpeedBonus * bonusScaler;

		// +8 defense
		player.statDefense += (int)(DefenseBonus * bonusScaler);

		// +10% pick speed
		player.pickSpeed -= PickSpeedBonus * bonusScaler;
	}
}
