using Everglow.SpellAndSkull.Items;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RedAlgaeMagicSpellBook : SpellTomeItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.damage = 70;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 4;
		Item.width = 38;
		Item.height = 36;
		Item.useTime = 4;
		Item.useAnimation = 4;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 0.5f;
		Item.value = 35000;
		Item.rare = ItemRarityID.Orange;
		Item.UseSound = SoundID.Item42;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<RedAlgaeMagicSpellBook_proj>();
		Item.shootSpeed = 0.5f;

		// DecorativeProjectileTypes.Add(ModContent.ProjectileType<MossySpellBook>());
		// DecorativeProjectileTypes.Add(ModContent.ProjectileType<MossySpellArray>());
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.GetModPlayer<SpellAndSkull.GlobalItems.MagicBookPlayer>().MagicBookLevel > 0)
		{
			return false;
		}
		if (player.ownedProjectileCounts[type] > 0)
		{
			foreach (var proj in Main.projectile)
			{
				if (proj is not null && proj.active && proj.type == type && proj.owner == player.whoAmI)
				{
					RedAlgaeMagicSpellBook_proj rAMSBp = proj.ModProjectile as RedAlgaeMagicSpellBook_proj;
					if (rAMSBp is not null)
					{
						if (!rAMSBp.Released)
						{
							return false;
						}
					}
				}
			}
		}
		Projectile.NewProjectileDirect(source, position, Vector2.zeroVector, type, damage, knockback, player.whoAmI);
		return false;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 18)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
}
