using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class GreenSungloStaff : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 28;
		Item.knockBack = 2f;
		Item.mana = 12;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item20;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;
		Item.autoReuse = false;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(0, 2, 0, 0);

		Item.shoot = ModContent.ProjectileType<GreenSungloSpore>();
		Item.shootSpeed = 15;
	}

	public override void HoldItem(Player player)
	{
		if (player.whoAmI != Main.myPlayer)
		{
			return;
		}

		player.ListenMouseWorld();

		if (player.itemAnimation <= 0)
		{
			// Back
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GreenSungloShield_B>()] <= 0)
			{
				Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GreenSungloShield_B>(), 0, 0, player.whoAmI);
			}

			// Front
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GreenSungloShield_A>()] <= 0)
			{
				Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GreenSungloShield_A>(), Item.damage, Item.knockBack, player.whoAmI);
			}
		}
	}
}
