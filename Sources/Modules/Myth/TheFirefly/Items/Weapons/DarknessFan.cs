using Everglow.Myth;
using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Items.Accessories;
using Everglow.Myth.TheFirefly.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class DarknessFan : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
	public override void SetDefaults()
	{

		Item.damage = 9;
		Item.DamageType = DamageClass.Summon;
		Item.mana = 12;
		Item.width = 74;
		Item.height = 90;
		Item.useTime = 36;
		Item.useAnimation = 36;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.knockBack = 4f;
		Item.value = Item.sellPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.LightRed;
		Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<GlowingButterfly>();
		Item.shootSpeed = 8;
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2 && colling == 0)
		{
			colling = 120;
			Projectile.NewProjectile(source, position + new Vector2(0, -24), velocity * 3.4f, ModContent.ProjectileType<DarkFanFly>(), (int)(damage * 1.4), knockback, player.whoAmI, 6 + player.maxMinions * 1.5f, 0f);
			Item.useTime = 6;
			Item.useAnimation = 6;
			// Item.UseSound = SoundID.DD2_JavelinThrowersAttack;
			return false;
		}
		type = ModContent.ProjectileType<DarkFan>();
		Projectile.NewProjectile(source, position + new Vector2(0, -24), velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
		Item.useTime = 36;
		Item.useAnimation = 36;
		return false;
	}

	private int colling = 0;

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2 && colling == 0)
		{
			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.UseSound = SoundID.DD2_JavelinThrowersAttack;
		}
		else
		{
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
		}
		return true;
	}
	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{
			if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
			{
				tooltips.AddRange(new TooltipLine[]
				{
					new(ModIns.Mod, "MothEyeBonusText", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeBonusText")),
					new(ModIns.Mod, "MothEyeDFanBonus", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextMothFan")),
				});
			}
		}
	}
	public override void UpdateInventory(Player player)
	{
		if (colling > 0)
		{
			colling--;
		}
		else
		{
			colling = 0;
		}
	}
}
