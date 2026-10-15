using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class StarDancer_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 14;
		Item.value = 50000;
		Item.rare = ItemRarityID.Green;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 25f / 14f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.StarDancer>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.StarDancer_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.FallenStar, 20)
			.AddIngredient(ItemID.SunplateBlock, 50)
			.AddTile(TileID.SkyMill)
			.Register();
	}
}
