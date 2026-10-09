using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CactusClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 7;
		Item.value = 68;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CactusClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CactusClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cactus, 18)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
