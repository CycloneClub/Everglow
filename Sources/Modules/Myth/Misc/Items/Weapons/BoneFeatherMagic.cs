using Everglow.Myth.Misc.Projectiles.Weapon.Magic.BoneFeatherMagic;
using Everglow.Myth.Misc.Projectiles.Weapon.Magic.FreezeFeatherMagic;
using Everglow.SpellAndSkull.GlobalItems;
using Everglow.SpellAndSkull.Items;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.Misc.Items.Weapons;

public class BoneFeatherMagic : SpellTomeItem
{
	// 骨羽会插在怪物身上10s
	// 每一根骨羽会增加下次攻击7%的伤害上不封顶
	// 增伤会消耗骨羽
	// 骨羽自身的攻击只会消耗附着1s以上的骨羽
	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}
	public override void SetDefaults()
	{
		Item.damage = 40;
		Item.DamageType = DamageClass.Magic;
		Item.width = 28;
		Item.height = 30;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 8;
		Item.value = 2512;
		Item.rare = ItemRarityID.LightRed;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.Magic.BoneFeather>();
		Item.shootSpeed = 4;
		Item.crit = 16;
		Item.mana = 12;

		DecorativeProjectileTypes.Add(ModContent.ProjectileType<BoneFeatherMagicBook>());
		DecorativeProjectileTypes.Add(ModContent.ProjectileType<BoneFeatherMagicArray>());
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
			.AddIngredient(ItemID.BoneFeather, 3)
			.AddTile(TileID.CrystalBall)
			.Register();
	}
}
