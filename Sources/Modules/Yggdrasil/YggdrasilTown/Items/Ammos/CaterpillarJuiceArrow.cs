using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
namespace Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;

public class CaterpillarJuiceArrow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 52;

		Item.damage = 6;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2f;

		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.ammo = AmmoID.Arrow;

		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(copper: 4);

		Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.CaterpillarJuiceArrow>();
		Item.shootSpeed = 6f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(30);
		recipe.AddIngredient(ItemID.WoodenArrow, 30);
		recipe.AddIngredient<CaterpillarJuice>(1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
