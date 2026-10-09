using Everglow.Myth.TheFirefly.Dusts;
using Everglow.Myth.TheFirefly.Items.Accessories;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class PhosphorescenceGun : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
	public override void SetStaticDefaults()
	{

	}

	public override void SetDefaults()
	{


		Item.width = 70;
		Item.height = 40;
		Item.rare = ItemRarityID.Green;
		Item.value = 2000;

		Item.useTime = 45;
		Item.useAnimation = 45;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;
		Item.UseSound = SoundID.Item36;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 10;
		Item.knockBack = 6f;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shoot = ProjectileID.PurificationPowder;
		Item.shootSpeed = 10f;
		Item.useAmmo = AmmoID.Bullet;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, 100).RotatedByRandom(6.283);
		const int NumProjectiles = 4;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.PhosphorescenceGun>()] < 1)
			Projectile.NewProjectileDirect(source, position + velocity * 2.0f - new Vector2(0, 4), Vector2.Zero, ModContent.ProjectileType<Projectiles.PhosphorescenceGun>(), damage, knockback, player.whoAmI, 1f, Item.useAnimation);
		else
		{
			for (int x = 0; x < Main.projectile.Length; x++)
			{
				if (Main.projectile[x].active)
				{
					if (Main.projectile[x].type == ModContent.ProjectileType<Projectiles.PhosphorescenceGun>())
					{
						if (Main.projectile[x].owner == player.whoAmI)
						{
							Main.projectile[x].ai[0] = 1f;
							Main.projectile[x].ai[1] = Item.useAnimation;
							if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
							{
								if (!mothEyePlayer.MothEyeEquipped && !fireflyBiome.IsBiomeActive(Main.LocalPlayer) && !Main.hardMode)
								{
									player.velocity -= velocity * 0.2f;
								}
							}
						}
					}
				}
			}
		}
		for (int i = 0; i < NumProjectiles; i++)
		{
			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
			newVelocity *= 1f - Main.rand.NextFloat(0.3f);
			Projectile.NewProjectileDirect(source, position + velocity * 2.0f - new Vector2(0, 4), newVelocity, type, damage, knockback, player.whoAmI);
		}
		for (int i = 0; i < NumProjectiles * 3; i++)
		{
			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
			newVelocity *= 1f - Main.rand.NextFloat(0.3f);
			if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
			{
				if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
					Projectile.NewProjectileDirect(source, position + velocity * 2.0f - new Vector2(0, 4), newVelocity, ModContent.ProjectileType<PhosphorescenceBullet>(), (int)(damage * 0.39f), knockback, player.whoAmI);
				else
				{
					Projectile.NewProjectileDirect(source, position + velocity * 2.0f - new Vector2(0, 4), newVelocity, ModContent.ProjectileType<PhosphorescenceBullet>(), (int)(damage * 0.26f), knockback, player.whoAmI);
				}
			}

			Vector2 basePos = position + newVelocity * 3.7f - new Vector2(0, 4);
			for (int z = 0; z < 3; z++)
			{
				Vector2 v = newVelocity * z / 3f;
				Dust.NewDust(basePos, 0, 0, ModContent.DustType<MothBlue>(), v.X, v.Y, 0, default, Main.rand.NextFloat(0.8f, 1.7f));
				v = newVelocity * (z + 0.5f) / 3f;
				Dust.NewDust(basePos, 0, 0, ModContent.DustType<MothBlue2>(), v.X, v.Y, 0, default, Main.rand.NextFloat(0.8f, 1.7f));
			}
			for (int j = 0; j < 9; j++)
			{
				Vector2 v = newVelocity / 27f * j;
				Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
				int num20 = Dust.NewDust(basePos - new Vector2(8), 0, 0, ModContent.DustType<BlueGlowAppear>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * 0.4f);
				Main.dust[num20].noGravity = true;
			}
			for (int j = 0; j < 18; j++)
			{
				Vector2 v = newVelocity / 54f * j;
				Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
				int num21 = Dust.NewDust(basePos - new Vector2(8), 0, 0, ModContent.DustType<BlueParticleDark2>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f));
				Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
			}
		}
		return false;
	}
	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{
			if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
			{
				tooltips.AddRange(new TooltipLine[]
				{
					new(ModIns.Mod, "MothEyeBonusText", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeBonusText")),
					new(ModIns.Mod, "MothEyeGunBonus0", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextMothGun0")),
					new(ModIns.Mod, "MothEyeGunBonus1", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextMothGun1")),
				});
			}
		}
	}
}
