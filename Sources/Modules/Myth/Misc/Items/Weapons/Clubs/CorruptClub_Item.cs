using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CorruptClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 30;
		Item.value = Item.buyPrice(0, 1, 35, 0);
		Item.rare = ItemRarityID.Blue;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 16f / 30f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CorruptClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CorruptClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 12)
			.AddIngredient(ItemID.ShadowScale, 4)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
