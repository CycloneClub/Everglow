using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PearlwoodClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 44;
		Item.value = Item.buyPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 7f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 30f / 44f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PearlwoodClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PearlwoodClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Pearlwood, 8)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
