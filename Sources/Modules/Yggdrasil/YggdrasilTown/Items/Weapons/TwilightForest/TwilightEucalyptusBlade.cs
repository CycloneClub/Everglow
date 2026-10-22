using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class TwilightEucalyptusBlade : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 64;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 60;
		Item.knockBack = 0.4f;
		Item.crit = 4;

		Item.useTime = Item.useAnimation = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(20);
		recipe.AddIngredient<TwilightCrystal>(12);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
