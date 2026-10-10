using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class BorealWoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 6;
		Item.value = 54;
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
