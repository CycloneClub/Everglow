using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;

public class LightArrow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 52;

		Item.damage = 6;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 4f;

		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.ammo = AmmoID.Arrow;

		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(copper: 3);

		Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.LightArrow>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(30);
		recipe.AddIngredient(ItemID.MusketBall, 30);
		recipe.AddIngredient<LampFruit>(1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
