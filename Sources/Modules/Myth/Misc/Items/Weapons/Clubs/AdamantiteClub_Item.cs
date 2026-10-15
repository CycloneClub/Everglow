using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class AdamantiteClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 36;
		Item.value = 138000;
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 61f / 36f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.AdamantiteClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.AdamantiteClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.AdamantiteBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
