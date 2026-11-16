using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class MossyCyatheaBow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 34;
		Item.rare = ItemRarityID.White;
		Item.noMelee = true;
		Item.DamageType = DamageClass.Ranged;
		Item.UseSound = SoundID.Item5;
		Item.autoReuse = false;
		Item.value = 1200;
		Item.useTime = 28;
		Item.useAnimation = 28;
		Item.damage = 16;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
		Item.useAmmo = AmmoID.Arrow;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (type == ProjectileID.WoodenArrowFriendly)
		{
			type = ModContent.ProjectileType<CyatheaArrow_Proj>();
		}
		int count = 1;
		if (Main.rand.NextBool(3))
		{
			count = 3;
		}
		for (int i = 0; i < count; i++)
		{
			var positionSky = player.Center + new Vector2(0, -1500) + new Vector2(0, 300).RotatedByRandom(MathHelper.TwoPi);
			var vel = (Main.MouseWorld - positionSky).NormalizeSafe() * velocity.Length();
			var posCheck = positionSky;
			var velCheck = vel;

			// parabolic prejudgement
			for (int t = 0; t < 300; t++)
			{
				posCheck += velCheck;
				velCheck += new Vector2(0, 0.1f);
				if (velCheck.Y > 16f)
				{
					velCheck.Y = 16f;
				}
				if (posCheck.Y >= Main.MouseWorld.Y)
				{
					vel.X -= (posCheck.X - Main.MouseWorld.X) / (float)t;
					break;
				}
			}
			Projectile.NewProjectileDirect(source, positionSky, vel, type, damage, knockback, player.whoAmI);
		}
		return false;
	}

	public override Vector2? HoldoutOffset()
	{
		return new Vector2(0, 0);
	}

	// public override void AddRecipes()
	// {
	// CreateRecipe()
	// .AddIngredient(ModContent.ItemType<CyanVineBar>(), 12)
	// .AddIngredient(ModContent.ItemType<StoneDragonScaleWood>(), 6)
	// .AddTile(TileID.WorkBenches)
	// .Register();
	// }
}
