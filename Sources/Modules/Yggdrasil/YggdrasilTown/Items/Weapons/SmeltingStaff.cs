using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class SmeltingStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 45;
		Item.height = 45;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.damage = 32;
		Item.knockBack = 2.4f;
		Item.mana = 10;
		Item.noMelee = true;
		Item.DamageType = DamageClass.Magic;
		Item.holdStyle = ItemHoldStyleID.HoldGuitar;
		Item.shoot = ModContent.ProjectileType<MeltingFireExplode>();
		Item.UseSound = SoundID.Item20;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 2);
	}

	public override void HoldItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<MeltingFireRing>()] == 0)
		{
			Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<MeltingFireRing>(), 0, 0, player.whoAmI);
		}
		base.HoldItem(player);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Vector2 pos = player.Center + player.DirectionTo(Main.MouseWorld) * (float)Math.Min(200, (Main.MouseWorld - player.Center).Length());
		Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos, Vector2.Zero, type, damage, knockback, player.whoAmI);
		return false;
	}
}
