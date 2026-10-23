using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class TwilightShamanStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 36;
		Item.height = 34;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 30;
		Item.mana = 10;
		Item.knockBack = 0.4f;
		Item.crit = 4;

		Item.useTime = Item.useAnimation = 22;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item20;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.shoot = ProjectileID.BallofFrost;
		Item.shootSpeed = 10;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(10);
		recipe.AddIngredient<TwilightCrystal>(12);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
