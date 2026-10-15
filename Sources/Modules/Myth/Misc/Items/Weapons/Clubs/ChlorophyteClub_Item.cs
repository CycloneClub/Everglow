using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class ChlorophyteClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 60;
		Item.value = 276000;
		Item.rare = ItemRarityID.Lime;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 80f / 60f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.ChlorophyteClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.ChlorophyteClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.ChlorophyteBar, 18)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
