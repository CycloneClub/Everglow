using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CorruptClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 12;
		Item.value = 169;
		Item.rare = ItemRarityID.Blue;
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
