using Everglow.Myth.TheFirefly.Items.Accessories;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class EvilChrysalis : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	private FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.damage = 24;
		Item.mana = 6;
		Item.width = 50;
		Item.height = 50;
		Item.useTime = 36;
		Item.useAnimation = 36;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.knockBack = 2.25f;
		Item.value = 2100;
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item44;
		Item.shoot = ModContent.ProjectileType<Projectiles.EvilChrysalis>();
		Item.autoReuse = true;
		Item.shootSpeed = 10f;
		Item.DamageType = DamageClass.Summon;
		Item.noUseGraphic = true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(0, 200f), Main.rand.NextFloat(0, 200f));
		return false;
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse == 2 && player.statMana >= player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()] && player.ownedProjectileCounts[ModContent.ProjectileType<MothMagicArray>()] == 0)
				{
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EvilChrysalisRightClick>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MothMagicArray>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
					return false;
				}
			}
		}
		return base.CanUseItem(player);
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
					new(ModIns.Mod, "MothEyeEChryBonus", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextMothChry0")),
					new(ModIns.Mod, "MothEyeEChryBonus", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextMothChry1")),
				});
			}
		}
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}
}
