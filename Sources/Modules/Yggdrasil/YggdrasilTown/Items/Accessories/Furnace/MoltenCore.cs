using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;

public class MoltenCore : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
		5,
		DefenseBonus,
		CritBonus,
		DamageBonus,
		MoveSpeedBonus,
		JumpSpeedBonus / 100f);

	private const int DefenseBonus = 5;
	private const int CritBonus = 5;
	private const int DamageBonus = 5;
	private const int MoveSpeedBonus = 5;
	private const int JumpSpeedBonus = 5;

	public override void SetStaticDefaults()
	{
		Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
	}

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.accessory = true;
		Item.rare = ItemRarityID.Lime;
		Item.value = Item.buyPrice(gold: 10);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// Bonuses while affected by OnFire, OnFire3 or CursedInferno.
		if (HasFireDebuff(player))
		{
			// +5% damage reduction
			player.endurance += 0.05f;
			// +5 defense
			player.statDefense += DefenseBonus;
			// +5% critical strike chance
			player.GetCritChance(DamageClass.Generic) += CritBonus;
			// +5% damage
			player.GetDamage(DamageClass.Generic) += DamageBonus / 100f;
			// +5% movement speed
			player.moveSpeed += MoveSpeedBonus / 100f;
			// +0.05 pixels/tick jump speed
			player.jumpSpeedBoost += JumpSpeedBonus / 100f;
		}
	}

	private static bool HasFireDebuff(Player player) => player.HasBuff(BuffID.OnFire) || player.HasBuff(BuffID.OnFire3) || player.HasBuff(BuffID.CursedInferno);

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture = ModAsset.MoltenCore.Value;
		int frameWidth = texture.Width;
		int frameHeight = texture.Height / 4;
		int frameOffsetY = frameHeight * (((int)Main.time / 5) % 4);

		spriteBatch.Draw(texture, Item.Center - Main.screenPosition, new Rectangle(0, frameOffsetY, frameWidth, frameHeight), lightColor, 0, new Vector2(frameWidth, frameHeight / 2) / 2, 1, SpriteEffects.None, 0);

		return false;
	}
}
