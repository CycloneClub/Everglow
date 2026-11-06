using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;

public class LightBullet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 6;
		Item.knockBack = 4;

		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(copper: 3);

		Item.ammo = AmmoID.Bullet;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.LightBullet>();
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
