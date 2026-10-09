using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PlatinumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 11;
		Item.value = 147;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PlatinumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PlatinumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.PlatinumBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
