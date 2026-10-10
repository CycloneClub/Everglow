using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class TungstenClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 9;
		Item.value = 112;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TungstenClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TungstenClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.TungstenBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
