using Everglow.Commons.Templates.Weapons.Gyroscopes;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class LegumeGyroscope : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 17;
		Item.knockBack = 0.2f;

		Item.useTime = Item.useAnimation = 16;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item117;
		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.channel = true;

		Item.value = 11700;
		Item.rare = ItemRarityID.Green;

		Item.shoot = ModContent.ProjectileType<LegumeGyroscope_Proj>();
		Item.shootSpeed = 0;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2)
		{
			var gyroscopePlayer = player.GetModPlayer<GyroscopePlayer>();
			if (gyroscopePlayer != null)
			{
				gyroscopePlayer.EnablePowerBarUI ^= true;
				if (gyroscopePlayer.EnablePowerBarUI)
				{
					CombatText.NewText(player.Hitbox, Color.White, "Enable Gyroscope Power Bar");
				}
				else
				{
					CombatText.NewText(player.Hitbox, Color.White, "Disable Gyroscope Power Bar");
				}
			}
			return false;
		}
		player.AddBuff(ModContent.BuffType<LegumeGyroscopeBuff>(), 1800000);
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}
}
