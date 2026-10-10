using Everglow.Myth.Misc.Projectiles.Weapon.Melee;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons;

public class CyanFrost : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.damage = 30;
		Item.DamageType = DamageClass.Melee;
		Item.width = 104;
		Item.height = 104;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 2;
		Item.value = 10000;
		Item.rare = ItemRarityID.LightRed;
		Item.UseSound = SoundID.Item1;
		Item.shoot = ModContent.ProjectileType<CyanFrostProj>();
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shootsEveryUse = true;
		Item.autoReuse = true;
		Item.crit = 16;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
		Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
		NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // Sync the changes in multiplayer.

		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}

	public override void AddRecipes()
	{
		CreateRecipe().AddIngredient(ItemID.CobaltBar, 6).AddIngredient(ItemID.IceBlade).AddIngredient(ItemID.FrostCore).AddTile(TileID.MythrilAnvil).Register();
		CreateRecipe().AddIngredient(ItemID.PalladiumBar, 6).AddIngredient(ItemID.IceBlade).AddIngredient(ItemID.FrostCore).AddTile(TileID.MythrilAnvil).Register();
	}
}
