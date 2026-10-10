using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PearlwoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 9;
		Item.value = 111;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PearlwoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PearlwoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Pearlwood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
