using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Auburn;

public class AuburnBell : ModItem
{
	override public string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 40;
		Item.height = 52;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 10);

		Item.DamageType = DamageClass.Summon;
		Item.damage = 14;
		Item.knockBack = 0.5f;
		Item.mana = 18;
		Item.crit = 0;

		Item.useTime = Item.useAnimation = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.shootSpeed = 10f;
		Item.shoot = ModContent.ProjectileType<AuburnBellMinion>();
	}

	public override bool Shoot(
		Player player,
		EntitySource_ItemUse_WithAmmo source,
		Vector2 position,
		Vector2 velocity,
		int type,
		int damage,
		float knockback)
	{
		if (player.numMinions >= player.maxMinions)
		{
			return false;
		}

		player.AddBuff(ModContent.BuffType<Buffs.AuburnBell>(), 2);
		int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.ownedProjectileCounts[type] + 1);
		if (Main.projectile.IndexInRange(p))
		{
			Main.projectile[p].originalDamage = Item.damage;
		}

		int ai0 = 1;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.type == type)
			{
				if (proj.owner == player.whoAmI)
				{
					proj.ai[0] = ai0;
					ai0++;
				}
			}
		}
		return false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<RodWing>(8);
		recipe.AddIngredient<LampWood_Wood>(10);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
