using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class IronClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 18;
		Item.value = Item.buyPrice(0, 0, 18, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 5.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 12f / 18f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.IronClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.IronClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.IronBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
