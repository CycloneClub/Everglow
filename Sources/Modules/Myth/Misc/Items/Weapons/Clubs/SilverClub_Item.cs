using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class SilverClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 8;
		Item.value = 108;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SilverClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SilverClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.SilverBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
