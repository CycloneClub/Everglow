using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class OrichalcumClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 34;
		Item.value = Item.buyPrice(0, 12, 65, 0);
		Item.rare = ItemRarityID.LightRed;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 59f / 34f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.OrichalcumClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.OrichalcumClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.OrichalcumBar, 14)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
