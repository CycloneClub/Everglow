using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Items.Weapons;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class MothEye : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

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
		player.maxMinions += 1;
		player.maxTurrets += 1;
		player.GetDamage(DamageClass.Summon) *= 1.06f;
		if (fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
		{
			player.manaSickReduction += 4;
			player.manaCost -= 0.05f;
		}
	}
	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
		{
			tooltips.AddRange(new TooltipLine[]
			{
				new(ModIns.Mod, "MothEyeText0", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeText0")),
				new(ModIns.Mod, "MothEyeText1", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeText1")),
				new(ModIns.Mod, "MothEyeText2", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeText2")),
				new(ModIns.Mod, "MothEyeText3", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeText3")),
				new(ModIns.Mod, "MothEyeText4", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeText4")),
			}); // Using \n would cause spacing problems in the tooltip section (blank space underneath all tooltips). ~Setnour6
		}
		else if (fireflyBiome.IsBiomeActive(Main.LocalPlayer))
		{
			tooltips.Add(new TooltipLine(ModIns.Mod, "MothEyeCriteriaText", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeCriteriaText")));
		}
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
	private FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
	public bool MothEyeEquipped;

	public override void ResetEffects()
	{
		MothEyeEquipped = false;
	}
	public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
	{
		if (fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
		{
			for (int f = 0; f < Player.armor.Length; f++)
			{
				if (Player.armor[f].type != ModContent.ItemType<MothEye>())
				{
					continue;
				}

				int[] FireflyWeapon =
				{
					ModContent.ItemType<DarknessFan>(),
					ModContent.ItemType<DreamWeaver>(), // no MothEye effect
					ModContent.ItemType<DustOfCorrupt>(), // no MothEye effect
					ModContent.ItemType<EvilChrysalis>(),
					ModContent.ItemType<FlowLightMissile>(), // no MothEye effect
					ModContent.ItemType<GlowBeadGun>(), // no MothEye effect
					ModContent.ItemType<GlowWoodSword>(),
					ModContent.ItemType<MothYoyo>(),
					ModContent.ItemType<NavyThunder>(), // no MothEye effect
					ModContent.ItemType<PhosphorescenceGun>(),
					ModContent.ItemType<ScaleWingBlade>(),
					ModContent.ItemType<ShadowWingBow>(),
				};
				if (Array.IndexOf(FireflyWeapon, item.type) != -1)
				{
					damage *= 1.05f;
				}

				break;
			}
		}
	}
}
// TODO: Finish Item Equip Effects (Displays a different equip texture when in the Firefly Biome, See MothEye_Neck.png and MothEye_NeckOff.png
