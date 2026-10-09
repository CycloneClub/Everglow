using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CopperClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 6;
		Item.value = 65;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CopperClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CopperClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.CopperBar, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
