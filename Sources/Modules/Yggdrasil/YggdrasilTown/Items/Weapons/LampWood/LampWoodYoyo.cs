using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Commons.Templates.Weapons.Yoyos;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LampWood;

public class LampWoodYoyo : YoyoItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 14;
		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(silver: 5);
		Item.shoot = ModContent.ProjectileType<Projectiles.Melee.LampWoodYoyo>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(10);
		recipe.AddIngredient(ItemID.Cobweb, 20);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
