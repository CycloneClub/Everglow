using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CactusClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 8;
		Item.value = 1800;
		Item.rare = ItemRarityID.White;
		Item.knockBack = 4.5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 10f / 8f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CactusClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CactusClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cactus, 18)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
