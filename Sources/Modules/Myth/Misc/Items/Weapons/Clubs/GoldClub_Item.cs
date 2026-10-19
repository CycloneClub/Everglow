using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class GoldClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 25;
		Item.value = Item.buyPrice(0, 0, 90, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 15f / 25f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.GoldClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.GoldClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.GoldBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
