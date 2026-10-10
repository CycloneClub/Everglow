using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PalladiumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 44;
		Item.value = 2074;
		Item.rare = ItemRarityID.LightRed;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalladiumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalladiumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.PalladiumBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
