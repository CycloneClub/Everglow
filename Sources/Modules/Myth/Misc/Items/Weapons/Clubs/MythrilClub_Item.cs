using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class MythrilClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 34;
		Item.value = 103500;
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 50f / 34f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.MythrilClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.MythrilClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.MythrilBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
