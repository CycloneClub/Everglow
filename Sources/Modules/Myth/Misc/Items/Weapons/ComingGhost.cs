using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.Misc.Items.Weapons;

public class ComingGhost : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.damage = 57;
		Item.DamageType = DamageClass.Melee;
		Item.width = 54;
		Item.height = 58;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.knockBack = 6;
		Item.value = 20000;
		Item.rare = ItemRarityID.Pink;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.Melee.ComingGhost>();
		Item.shootSpeed = 8;
		Item.crit = 8;
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
			Projectile.NewProjectile(source, position + new Vector2(0, -24), velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
		}
		return false;
	}
}
