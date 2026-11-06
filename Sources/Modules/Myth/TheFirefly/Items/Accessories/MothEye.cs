using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Items.Weapons;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class MothEye : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(8, 1, 1, 5, 5, 4);

	private FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();

	public static Player LocalOwner => Main.LocalPlayer;

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.value = 2000;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		// Item.vanity = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<MothEyePlayer>().MothEyeEquipped = true;
		// +1 max minion
		player.maxMinions += 1;
		// +1 max sentry
		player.maxTurrets += 1;
		// +8% summon damage
		player.GetDamage(DamageClass.Summon) *= 1.08f;
		// In the Firefly biome during Hardmode:
		if (HasBiomeBonus(player))
		{
			// +4 Mana Sickness penalty
			// TODO: Confirm intended effect
			player.manaSickReduction += 4;
			// -5% mana cost
			player.manaCost -= 0.05f;
		}
	}

	private static bool HasBiomeBonus(Player player) => Main.hardMode && ModContent.GetInstance<FireflyBiome>().IsBiomeActive(player);

	// In the Firefly biome during Hardmode:
	// +5% damage with Firefly weapons
	internal static float CalculateWeaponDamage(Player player, Item item)
	{
		if (!HasBiomeBonus(player))
		{
			return 1;
		}

		int[] fireflyWeapons =
		{
			ModContent.ItemType<DarknessFan>(),
			ModContent.ItemType<DreamWeaver>(),
			ModContent.ItemType<DustOfCorrupt>(),
			ModContent.ItemType<EvilChrysalis>(),
			ModContent.ItemType<FlowLightMissile>(),
			ModContent.ItemType<GlowBeadGun>(),
			ModContent.ItemType<GlowWoodSword>(),
			ModContent.ItemType<MothYoyo>(),
			ModContent.ItemType<NavyThunder>(),
			ModContent.ItemType<PhosphorescenceGun>(),
			ModContent.ItemType<ScaleWingBlade>(),
			ModContent.ItemType<ShadowWingBow>(),
		};
		return Array.IndexOf(fireflyWeapons, item.type) != -1 ? 1.05f : 1;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
		{
			Texture2D mEyeTex = ModAsset.MothEye_GlowOn.Value;
			for (int x = 0; x < 8; x++)
			{
				Vector2 v0 = new Vector2(0, 6 + 2f * (float)Math.Sin(Main.timeForVisualEffects * 0.1)).RotatedBy(x / 4d * Math.PI);
				spriteBatch.Draw(mEyeTex, position + v0, null, new Color(0.2f, 0.2f, 0.2f, 0), 0f, origin, scale, 0, 0f);
			}
			spriteBatch.Draw(mEyeTex, position, null, drawColor, 0f, origin, scale, 0, 0f);
		}
		else
		{
			Texture2D mEyeTex = ModAsset.MothEye_GlowOff.Value;
			for (int x = 0; x < 8; x++)
			{
				Vector2 v0 = new Vector2(0, 8 + 3f * (float)Main.timeForVisualEffects).RotatedBy(x / 4d * Math.PI);
				spriteBatch.Draw(mEyeTex, position + v0 * 15, null, new Color(1f, 1f, 1f, 0), 0f, origin, scale, 0, 0f);
			}
			spriteBatch.Draw(mEyeTex, position, null, drawColor, 0f, origin, scale, 0, 0f);
		}
	}
	// TODO:DIDNOT FINISH Equipped Effect:Change texture in Firefly biome, fail.
}

internal class MothEyePlayer : ModPlayer
{
	public bool MothEyeEquipped;

	public override void ResetEffects()
	{
		MothEyeEquipped = false;
	}

	public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
	{
		// While Moth Eye is equipped:
		if (MothEyeEquipped)
		{
			damage *= MothEye.CalculateWeaponDamage(Player, item);
		}
	}
}
// TODO: Finish Item Equip Effects (Displays a different equip texture when in the Firefly Biome, See MothEye_Neck.png and MothEye_NeckOff.png
