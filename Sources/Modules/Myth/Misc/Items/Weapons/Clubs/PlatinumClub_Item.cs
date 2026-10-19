using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PlatinumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 28;
		Item.value = Item.buyPrice(0, 1, 35, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 16f / 28f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PlatinumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PlatinumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.PlatinumBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
