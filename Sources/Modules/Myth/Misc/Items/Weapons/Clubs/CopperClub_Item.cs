using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CopperClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 13;
		Item.value = 450;
		Item.rare = ItemRarityID.White;
		Item.knockBack = 5.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 9f / 13f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CopperClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CopperClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.CopperBar, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
