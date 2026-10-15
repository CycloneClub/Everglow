using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class EbonwoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 12;
		Item.value = 100;
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 11f / 12f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.EbonwoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.EbonwoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Ebonwood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
