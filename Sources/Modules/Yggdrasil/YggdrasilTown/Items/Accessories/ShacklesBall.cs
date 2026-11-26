using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class ShacklesBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(EnduranceBonus * 100f, MoveSpeedReduction * 100f, JumpSpeedReduction * 100f, JumpHeightReduction * 100f);

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

		player.GetModPlayer<ShacklesBallPlayer>().Equipped = true;

		// 5. Immune to 'Slow' debuff
		player.buffImmune[BuffID.Slow] = true;
	}
}

public class ShacklesBallPlayer : ModPlayer
{
	public bool Equipped { get; set; }

	public override void ResetEffects() => Equipped = false;

	public override void PostUpdateMiscEffects()
	{
		// Vanilla applies moveSpeed only to the ordinary run cap. Keep the barefoot sprint cap in sync.
		if (Equipped && Player.accRunSpeed == Player.maxRunSpeed && Player.moveSpeed < 1f)
		{
			Player.accRunSpeed *= Math.Max(0f, Player.moveSpeed);
		}
	}

	public override void PostUpdateRunSpeeds()
	{
		if (Equipped && !Player.mount.Active)
		{
			Player.jumpHeight = (int)MathF.Round(Player.jumpHeight * (1f - ShacklesBall.JumpHeightReduction));
		}
	}
}
