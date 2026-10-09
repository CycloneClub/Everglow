using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class RichMahoganyClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 6;
		Item.value = 64;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.RichMahoganyClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.RichMahoganyClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.RichMahogany, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
