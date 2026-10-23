namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class ShacklesBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const float EnduranceBonus = 0.08f;
	public const float MoveSpeedReduction = 0.1f;
	public const float JumpSpeedReduction = 0.1f;
	public const float JumpHeightReduction = 0.05f;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.width = 28;
		Item.height = 20;

		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(platinum: 0, gold: 1);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. +8% damage reduction
		player.endurance += EnduranceBonus;

		// 2. -10% move speed
		player.moveSpeed -= MoveSpeedReduction;

		// 3. -10% jump speed
		player.jumpSpeedBoost -= Player.jumpSpeed * JumpSpeedReduction;

		// 4. -5% jump height
		player.jump = (int)(player.jump * (1 - JumpHeightReduction));

		// 5. Immune to 'Slow' debuff
		player.buffImmune[BuffID.Slow] = true;
	}
}
