using Everglow.Myth.Misc.Projectiles.Weapon.Magic.FireFeatherMagic;
using Everglow.SpellAndSkull.GlobalItems;
using Everglow.SpellAndSkull.Items;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.Misc.Items.Weapons;

public class FireFeatherMagic : SpellTomeItem
{
	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.damage = 36;
		Item.DamageType = DamageClass.Magic;
		Item.width = 28;
		Item.height = 30;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 8;
		Item.value = 2354;
		Item.rare = ItemRarityID.LightRed;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.Magic.FireFeather>();
		Item.shootSpeed = 4;
		Item.crit = 16;
		Item.mana = 12;

		DecorativeProjectileTypes.Add(ModContent.ProjectileType<FireFeatherMagicBook>());
		DecorativeProjectileTypes.Add(ModContent.ProjectileType<FireFeatherMagicArray>());
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.GetModPlayer<MagicBookPlayer>().MagicBookLevel == 1)
		{
			return false;
		}
		else
		{
			SoundEngine.PlaySound(SoundID.Item39, position);
			for (int k = 0; k < 3; k++)
			{
				Vector2 v2 = velocity.RotatedBy(Main.rand.NextFloat(-0.42f, 0.42f)) * Main.rand.NextFloat(0.9f, 1.1f);
				Projectile.NewProjectile(source, position + velocity * 2f, v2, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(1f));
			}
		}
		return false;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<FeatherMagic>()
			.AddIngredient(ItemID.FireFeather, 3)
			.AddTile(TileID.CrystalBall)
			.Register();
	}
}
