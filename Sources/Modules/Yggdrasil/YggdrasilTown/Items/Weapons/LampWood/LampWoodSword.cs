using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.Audio;
using Terraria.DataStructures;
namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LampWood;

public class LampWoodSword : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 66;
		Item.height = 68;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.knockBack = 4.5f;
		Item.damage = 11;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<LampWoodSword_Projectile>();

		Item.value = 410;
	}
	public override bool CanUseItem(Player player)
	{
		Item.useTime = (int)(18f / player.meleeSpeed);
		Item.useAnimation = (int)(18f / player.meleeSpeed);
		return player.ownedProjectileCounts[Item.shoot] < 1;
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{

		if (player.ownedProjectileCounts[Item.shoot] < 1)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
		}
		return false;
	}
	public override bool? UseItem(Player player)
	{
		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(Item.UseSound, player.Center);
		}

		return null;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<LampWood_Wood>(), 7)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
