using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class SilverClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 21;
		Item.value = Item.buyPrice(0, 0, 45, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 14f / 21f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SilverClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SilverClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.SilverBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
