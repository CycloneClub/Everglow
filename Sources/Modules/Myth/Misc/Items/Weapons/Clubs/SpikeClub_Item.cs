using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Myth.Misc.Items.Weapons.Clubs;

public class SpikeClub_Item : ClubItem
{
	public override void SetCustomDefaults()
	{
		Item.damage = 26;
		Item.value = 15000;
		Item.rare = ItemRarityID.Green;
		Item.knockBack = 6f;
		Item.useTime = 12;
		Item.useAnimation = 12;
		SmashDamageMultiplier = 24f / 26f;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SpikeClub>();
		ProjSmashType = ModContent.ProjectileType<Projectiles.Weapon.Melee.Clubs.SpikeClub_smash>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Spike, 114)
			.AddCondition(Condition.DownedSkeletron)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
