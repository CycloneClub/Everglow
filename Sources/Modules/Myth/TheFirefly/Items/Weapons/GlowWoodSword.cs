using Everglow.Myth.TheFirefly.Items.Accessories;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class GlowWoodSword : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	private FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.damage = 13;
		Item.DamageType = DamageClass.Melee;
		Item.width = 56;
		Item.height = 56;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 5f;
		Item.value = Item.sellPrice(0, 0, 0, 70);
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		if (player.itemAnimation % 4 == 2)
		{
			Vector2 v0 = new Vector2(-3 * player.direction, -7 * player.gravDir).RotatedBy((player.itemAnimationMax - player.itemAnimation) * 0.13 * player.direction * player.gravDir + Math.Sin(Main.timeForVisualEffects / 16) * 0.3);
			Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center + v0 * 3, v0, ModContent.ProjectileType<Projectiles.GlowWoodSword>(), (int)(Item.damage * 0.3f), Item.knockBack, player.whoAmI);
		}
		base.MeleeEffects(player, hitbox);
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
					new(ModIns.Mod, "MothEyeGlowSwordBonus0", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextGlowSword0")),
					new(ModIns.Mod, "MothEyeGlowSwordBonus1", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextGlowSword1")),
				});
			}
		}
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<GlowWood>(), 12);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
