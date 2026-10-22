using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LampWood;

public class LampWoodBow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 78;

		Item.damage = 8;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 0;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = 29;
		Item.useAnimation = 25;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.noUseGraphic = false;
		Item.noMelee = true;
		Item.channel = false;

		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
		Item.useAmmo = AmmoID.Arrow;

		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(silver: 1);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(10);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
