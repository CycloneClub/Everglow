using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class TinClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 6;
		Item.value = 70;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TinClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TinClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.TinBar, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
