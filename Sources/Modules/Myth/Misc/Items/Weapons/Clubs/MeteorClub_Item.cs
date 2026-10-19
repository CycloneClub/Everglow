using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class MeteorClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 15;
		Item.value = Item.buyPrice(0, 2, 70, 0);
		Item.rare = ItemRarityID.Blue;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 26f / 15f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.MeteorClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.MeteorClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.MeteoriteBar, 18)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
