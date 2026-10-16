using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class LeadClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 20;
		Item.value = Item.buyPrice(0, 0, 27, 0);
		Item.rare = ItemRarityID.White;
		Item.knockBack = 5.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 13f / 20f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.LeadClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.LeadClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.LeadBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
