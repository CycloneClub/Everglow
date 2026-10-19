using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CrimsonClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 32;
		Item.value = Item.buyPrice(0, 1, 35, 0);
		Item.rare = ItemRarityID.Blue;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 22f / 32f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CrimsonClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CrimsonClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 12)
			.AddIngredient(ItemID.TissueSample, 4)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
