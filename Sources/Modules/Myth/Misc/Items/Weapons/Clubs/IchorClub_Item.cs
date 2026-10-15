using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class IchorClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 24;
		Item.value = 200000;
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 4f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 30f / 24f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.IchorClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.IchorClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<CrimsonClub_Item>()
			.AddIngredient(ItemID.Ichor, 20)
			.AddIngredient(ItemID.SoulofNight, 10)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
