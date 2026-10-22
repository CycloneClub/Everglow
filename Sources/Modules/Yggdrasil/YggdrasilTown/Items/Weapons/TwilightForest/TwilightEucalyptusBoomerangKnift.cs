using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class TwilightEucalyptusBoomerangKnift : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 50;
		Item.height = 40;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 30;
		Item.knockBack = 0.4f;
		Item.crit = 4;

		Item.useTime = Item.useAnimation = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shoot = ModContent.ProjectileType<TwilightEucalyptusBoomerangKnift_Projectile>();
		Item.shootSpeed = 16;
	}

	public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 3;

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(15);
		recipe.AddIngredient<TwilightCrystal>(8);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
