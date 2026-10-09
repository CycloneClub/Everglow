using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class AdamantiteClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 74;
		Item.value = 3690;
		Item.rare = ItemRarityID.LightRed;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.AdamantiteClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.AdamantiteClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.AdamantiteBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
