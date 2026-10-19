using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class TinClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 15;
		Item.value = Item.buyPrice(0, 0, 6, 75);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 5.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 10f / 15f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TinClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TinClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.TinBar, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
