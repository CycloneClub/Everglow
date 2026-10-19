using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PalmWoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 9;
		Item.value = Item.buyPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 8f / 9f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalmWoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalmWoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.PalmWood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
