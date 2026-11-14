using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.DevilHeart;

public class DevilHeartStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public int UseItemCount = 0;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
		base.SetStaticDefaults();
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 17;
		Item.knockBack = 0.8f;
		Item.mana = 8;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item20;
		Item.useTime = Item.useAnimation = 16;
		Item.noMelee = true;
		Item.autoReuse = false;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(0, 2, 0, 0);

		Item.shoot = ModContent.ProjectileType<DevilHeartStaff_Proj>();
		Item.shootSpeed = 8;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		UseItemCount++;
		if (UseItemCount >= 7)
		{
			Projectile.NewProjectileDirect(source, position, velocity * 2.2f, ModContent.ProjectileType<DevilHeartStaff_Proj_II>(), (int)(damage * 2.5), knockback, player.whoAmI);
			UseItemCount = 0;
			return false;
		}
		return true;
	}
}
