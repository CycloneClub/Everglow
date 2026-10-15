using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class BorealWoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 9;
		Item.value = 100;
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 8f / 9f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.BorealWoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.BorealWoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.BorealWood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
