using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class CyanVineAxe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 46;
		Item.height = 50;
		Item.useAnimation = 25;
		Item.useTime = 25;
		Item.knockBack = 3.5f;
		Item.damage = 10;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;

		Item.value = 3300;

		Item.axe = 12;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<CyanVineBar>(8);
		recipe.AddIngredient<StoneDragonScaleWood>(3);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
