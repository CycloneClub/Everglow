using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.Misc.Items.Weapons;

public class FeatherMagic : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.damage = 12;
		Item.DamageType = DamageClass.Magic;
		Item.width = 28;
		Item.height = 30;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 1;
		Item.value = 800;
		Item.rare = ItemRarityID.Orange;
		Item.autoReuse = true;
		Item.shoot = ProjectileID.HarpyFeather;
		Item.shootSpeed = 5.3f;
		Item.crit = 3;
		Item.mana = 13;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Vector2 v = velocity;
		SoundEngine.PlaySound(SoundID.Item39, position);
		for (int k = 0; k < 4; k++)
		{
			Vector2 v2 = v.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.9f, 1.1f);
			int u = Projectile.NewProjectile(source, position + velocity * 2f, v2, type, damage, knockback, player.whoAmI, 0f);
			Main.projectile[u].hostile = false;
			Main.projectile[u].friendly = true;
			Main.projectile[u].penetrate = 2;
		}
		return false;
	}
}
