using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class WoodenClub_Item : ClubItem
{
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Wood, 18)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override void SetCustomDefaults()
	{
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.WoodenClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.WoodenClub_smash>();
	}
}
