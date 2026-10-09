using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class GoldClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 10;
		Item.value = 142;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.GoldClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.GoldClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.GoldBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
