using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PalmWoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 7;
		Item.value = 72;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalmWoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalmWoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.PalmWood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
