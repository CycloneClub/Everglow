using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class TungstenClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 22;
		Item.value = 6750;
		Item.rare = ItemRarityID.White;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 14f / 22f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TungstenClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.TungstenClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.TungstenBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
