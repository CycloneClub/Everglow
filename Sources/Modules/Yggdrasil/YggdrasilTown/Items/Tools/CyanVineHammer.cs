using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class CyanVineHammer : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 38;
		Item.height = 42;
		Item.useAnimation = 26;
		Item.useTime = 26;
		Item.knockBack = 4.5f;
		Item.damage = 9;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;

		Item.value = 3600;

		Item.hammer = 68;
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
