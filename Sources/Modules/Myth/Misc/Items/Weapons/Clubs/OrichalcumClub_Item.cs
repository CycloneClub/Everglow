using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class OrichalcumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 60;
		Item.value = 2717;
		Item.rare = ItemRarityID.LightRed;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.OrichalcumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.OrichalcumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.OrichalcumBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
