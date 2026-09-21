using Everglow.Yggdrasil.Common.Fish;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;
using ModLiquidLib.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.MidnightBayou;

public class KissOfCthulhu : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetStaticDefaults()
	{
		// 0.001% chance per tick, set this value according to vanilla npc Nymph.
		FishSystem.RegisterFish(ModContent.GetInstance<MidnightBayouBiome>(), new(Item.type, LiquidLoader.LiquidType<DarkSludgeLiquid>(), 0.0_0001f));
	}

	public override void SetDefaults()
	{
		Item.width = 94;
		Item.height = 32;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 21;
		Item.knockBack = 1.8f;
		Item.crit = 4;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = Item.useAnimation = 22;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 1, silver: 70);

		Item.shoot = ProjectileID.Bullet;
		Item.shootSpeed = 12;
		Item.useAmmo = AmmoID.Bullet;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		position += new Vector2((Item.width - 50) * Item.scale, 0).RotatedBy(velocity.ToRotation());

		// Shoot 2-4 normal bullet once
		int projNum = Main.rand.Next(2, 5);
		for (int i = 0; i < projNum; i++)
		{
			var projVelocity = velocity.RotatedBy((projNum / 2f - i) / 8f);
			Projectile.NewProjectile(source, position + velocity * 2, projVelocity, type, damage, knockback, player.whoAmI);
		}
		SoundEngine.PlaySound(SoundID.Item38);

		// The weapon has 1/3 chance to shoot a special projectile
		if (Main.rand.NextBool(3))
		{
			Projectile.NewProjectile(source, position + velocity * 2, velocity * 2.2f, ModContent.ProjectileType<KissOfCthulhu_Projectile>(), 1, knockback, player.whoAmI);
			SoundEngine.PlaySound(new SoundStyle(ModAsset.KissOfCthulhu_Shoot_Mod), player.Center);
		}

		return false;
	}

	public override Vector2? HoldoutOffset() => new Vector2(-26, -2);

	public override Vector2? HoldoutOrigin() => new Vector2(0, 0);

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		var centralTile = TileUtils.SafeGetTile(Item.Center.ToTileCoordinates());
		if (centralTile.LiquidType == LiquidLoader.LiquidType<DarkSludgeLiquid>())
		{
			if (Item.Center.Y % 16 > 16 - centralTile.LiquidAmount / 16f)
			{
				maxFallSpeed *= -0.4f;
			}
		}
		base.Update(ref gravity, ref maxFallSpeed);
	}
}
