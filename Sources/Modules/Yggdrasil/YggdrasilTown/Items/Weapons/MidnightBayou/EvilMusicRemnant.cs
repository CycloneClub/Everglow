using Everglow.Yggdrasil.Common.Fish;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;
using ModLiquidLib.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.MidnightBayou;

public class EvilMusicRemnant : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	public int UseCount = 0;

	public override void SetStaticDefaults()
	{
		// 0.001% chance per tick, set this value according to vanilla npc Nymph.
		FishSystem.RegisterFish(ModContent.GetInstance<MidnightBayouBiome>(), new(Item.type, LiquidLoader.LiquidType<DarkSludgeLiquid>(), 0.0_0001f));
	}

	public override void SetDefaults()
	{
		Item.width = 46;
		Item.height = 44;
		Item.scale = 0.8f;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 13;
		Item.knockBack = 1.1f;

		Item.rare = ItemRarityID.Green;
		Item.value = 14000;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAnimation = Item.useTime = 21;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.shoot = ModContent.ProjectileType<EvilMusicRemnant_Projectile>();
		Item.shootSpeed = 6;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		UseCount++;
		if (UseCount >= 7)
		{
			UseCount = 0;
		}
		float value = UseCount + MathF.Cos((UseCount * 2) * MathHelper.PiOver2);
		SoundEngine.PlaySound(SoundID.Item26.WithPitchOffset(UseCount / 7f - 0.5f), player.Center);

		// Player.slotsMinions remain 0 here, so we can only calculate it manually
		var projNum = (int)(player.maxMinions - player.GetSlotsMinions()) + 2;
		for (int i = 0; i < projNum; i++)
		{
			var projectile = Projectile.NewProjectileDirect(source, position + velocity, velocity.RotatedBy(Main.rand.NextFloat(-1, 1)), type, damage / 2, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;
		}
		Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.zeroVector, ModContent.ProjectileType<EvilMusicRemnant_Note_Mark>(), 0, 0, player.whoAmI);
		return false;
	}

	public override void HoldItem(Player player)
	{
		if (player.controlUseItem)
		{
			player.SetArmToFitMousePosition(0.1f);
		}
	}

	public override Vector2? HoldoutOffset() => new Vector2(6, -3);

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
