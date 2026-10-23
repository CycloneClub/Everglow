using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class ThermoprobeStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	public override void SetDefaults()
	{
		Item.width = 45;
		Item.height = 45;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.damage = 16;
		Item.knockBack = 1f;
		Item.mana = 12;
		Item.DamageType = DamageClass.Summon;
		Item.noMelee = true;
		Item.shoot = ModContent.ProjectileType<Thermoprobe>();
		Item.UseSound = SoundID.Item20;
		Item.value = Item.buyPrice(0, 2, 0, 0);
		Item.rare = ItemRarityID.Green;
	}

	public override bool CanUseItem(Player player)
	{
		return player.ownedProjectileCounts[Item.shoot] < player.maxMinions * 2;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Vector2 pos = player.Center + player.DirectionTo(Main.MouseWorld) * (float)Math.Min(200, (Main.MouseWorld - player.Center).Length());
		Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos, Vector2.Zero, type, damage, knockback, player.whoAmI, 4);
		return false;
	}
}
