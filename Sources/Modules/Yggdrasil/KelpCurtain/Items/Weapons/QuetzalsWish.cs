using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class QuetzalsWish : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public override string Texture => $"Terraria/Images/Item_{ItemID.TerraBlade}";

	public override void SetDefaults()
	{
		Item.width = 70;
		Item.height = 70;
		Item.damage = 37;
		Item.DamageType = DamageClass.Melee;
		Item.crit = 8;
		Item.knockBack = 6f;
		Item.useTime = 24;
		Item.useAnimation = 24;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;
		Item.channel = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<QuetzalsWishBlade>();
		Item.shootSpeed = 16f;
		Item.UseSound = SoundID.Item1;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 10);
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		foreach (var projectile in Main.ActiveProjectiles)
		{
			if (projectile.owner == player.whoAmI && projectile.ModProjectile is QuetzalsWishBlade blade && blade.IsHeld)
			{
				return false;
			}
		}
		Item.channel = player.altFunctionUse == 2;
		return true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			int stage = player.GetModPlayer<QuetzalsWishPlayer>().BeginAttack(player.altFunctionUse == 2);
			Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, stage, 0f, velocity.ToRotation());
		}
		return false;
	}
}
