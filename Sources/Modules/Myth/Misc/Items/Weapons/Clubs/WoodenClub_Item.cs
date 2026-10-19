using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class WoodenClub_Item : ClubItem
{
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Wood, 18)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override void SetCustomDefaults()
	{
		Item.damage = 8;
		Item.value = Item.buyPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 7f / 8f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.WoodenClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.WoodenClub_smash>();
	}
}
