using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CobaltClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 32;
		Item.value = Item.buyPrice(0, 6, 90, 0);
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 40f / 32f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CobaltClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CobaltClub_Smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.CobaltBar, 18)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
