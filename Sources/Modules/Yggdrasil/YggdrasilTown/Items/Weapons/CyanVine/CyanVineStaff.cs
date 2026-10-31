using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.CyanVine;

public class CyanVineStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 60;
		Item.height = 68;
		Item.useAnimation = 28;
		Item.useTime = 28;
		Item.knockBack = 4f;
		Item.damage = 15;
		Item.rare = ItemRarityID.White;

		Item.value = 4000;
		Item.autoReuse = false;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 6;
		Item.channel = true;

		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shoot = ModContent.ProjectileType<CyanVineStaff_proj>();
		Item.shootSpeed = 12;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<CyanVineBar>(), 16)
			.AddIngredient(ModContent.ItemType<StoneDragonScaleWood>(), 13)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override bool CanUseItem(Player player)
	{
		return player.ownedProjectileCounts[ModContent.ProjectileType<CyanVineStaff_proj>()] == 0;
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}
}
