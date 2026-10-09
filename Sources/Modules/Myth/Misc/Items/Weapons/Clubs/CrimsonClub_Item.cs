using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CrimsonClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 13;
		Item.value = 174;
		Item.rare = ItemRarityID.Blue;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CrimsonClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CrimsonClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 12)
			.AddIngredient(ItemID.TissueSample, 4)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
