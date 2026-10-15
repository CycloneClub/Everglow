using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CurseClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 30;
		Item.value = 200000;
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 6.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 55f / 30f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CurseClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CurseClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<CorruptClub_Item>()
			.AddIngredient(ItemID.CursedFlame, 20)
			.AddIngredient(ItemID.SoulofNight, 10)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
