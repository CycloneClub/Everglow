using Everglow.Myth.TheFirefly.Projectiles;
using Everglow.SpellAndSkull.Items;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class DreamWeaver : SpellTomeItem// TODO:织梦丝雨
{
	public override void SetDefaults()
	{
		Item.damage = 13;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 4;
		Item.width = 34;
		Item.height = 46;
		Item.useTime = 16;
		Item.useAnimation = 16;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 2.5f;
		Item.value = Item.sellPrice(0, 0, 20, 0);
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item42;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.DreamWeaver_proj>();
		Item.shootSpeed = 12f;
		DecorativeProjectileTypes.Add(ModContent.ProjectileType<TheFirefly.Projectiles.DreamWeaver.DreamWeaverBook>());
		DecorativeProjectileTypes.Add(ModContent.ProjectileType<TheFirefly.Projectiles.DreamWeaver.DreamWeaverArray>());
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.GetModPlayer<SpellAndSkull.GlobalItems.MagicBookPlayer>().MagicBookLevel > 0)
		{
			return false;
		}
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}

	public override void HoldItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<DreamWeaverBall>()] < 1 && player.GetModPlayer<SpellAndSkull.GlobalItems.MagicBookPlayer>().MagicBookLevel > 0)
		{
			var p0 = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center + new Vector2(0, 300), Vector2.zeroVector, ModContent.ProjectileType<DreamWeaverBall>(), player.HeldItem.damage * 2, player.HeldItem.knockBack, player.whoAmI);
			p0.CritChance = player.GetWeaponCrit(player.HeldItem);
			p0.scale = 0;
		}
		base.HoldItem(player);
	}
}
