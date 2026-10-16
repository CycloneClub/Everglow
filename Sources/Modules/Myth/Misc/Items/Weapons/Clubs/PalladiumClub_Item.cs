using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class PalladiumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 30;
		Item.value = Item.buyPrice(0, 9, 20, 0);
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 5.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 49f / 30f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalladiumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.PalladiumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.PalladiumBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
