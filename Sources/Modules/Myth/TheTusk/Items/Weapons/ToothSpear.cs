using Terraria.Audio;
using Terraria.DataStructures;
namespace Everglow.Myth.TheTusk.Items.Weapons;

public class ToothSpear : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Orange;
		Item.value = 5381;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAnimation = 24;
		Item.useTime = 24;
		Item.UseSound = SoundID.Item71;
		Item.autoReuse = true;

		Item.damage = 38;
		Item.crit = 12;
		Item.knockBack = 6.5f;
		Item.noUseGraphic = true;
		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;

		Item.shootSpeed = 3.7f;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.ToothSpear_proj>();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}
	public override bool? UseItem(Player player)
	{
		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(Item.UseSound, player.Center);
		}

		return null;
	}
}
