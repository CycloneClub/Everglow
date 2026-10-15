using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class CrystalClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 28;
		Item.value = 200000;
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 5f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 35f / 28f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CrystalClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.CrystalClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<PearlwoodClub_Item>()
			.AddIngredient(ItemID.CrystalShard, 20)
			.AddIngredient(ItemID.SoulofLight, 10)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
