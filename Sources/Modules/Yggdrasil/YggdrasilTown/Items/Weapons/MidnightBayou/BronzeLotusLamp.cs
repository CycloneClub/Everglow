using Everglow.Yggdrasil.Common.Fish;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using ModLiquidLib.ModLoader;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.MidnightBayou;

public class BronzeLotusLamp : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		// 0.005% chance per tick, set this value according to vanilla npc Nymph.
		FishSystem.RegisterFish(ModContent.GetInstance<MidnightBayouBiome>(), new(Item.type, LiquidLoader.LiquidType<DarkSludgeLiquid>(), 0.0_0005f));
	}

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 52;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 8;
		Item.knockBack = 0.7f;
		Item.mana = 6;

		Item.rare = ItemRarityID.Blue;
		Item.value = 27000;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = 2;
		Item.useTime = 2;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.channel = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shoot = ModContent.ProjectileType<BronzeLotusLamp_weapon>();
		Item.shootSpeed = 8;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}
}
