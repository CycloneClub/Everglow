using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class TitaniumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 80;
		Item.value = 3751;
		Item.rare = ItemRarityID.LightRed;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TitaniumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TitaniumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.TitaniumBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
