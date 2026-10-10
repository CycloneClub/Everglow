using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class SpikeClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 21;
		Item.value = 450;
		Item.rare = ItemRarityID.Green;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SpikeClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SpikeClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Spike, 114)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
