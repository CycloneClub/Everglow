using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class ShadewoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 7;
		Item.value = 80;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.ShadewoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.ShadewoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Shadewood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
