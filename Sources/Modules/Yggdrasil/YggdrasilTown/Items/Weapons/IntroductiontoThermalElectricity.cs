using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class IntroductiontoThermalElectricity : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 30;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 12;
		Item.knockBack = 2;
		Item.mana = 20;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = Item.useAnimation = 36;
		Item.noMelee = true;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Orange;
		Item.value = 80000;

		Item.shoot = ModContent.ProjectileType<IntroductiontoThermalElectricity_Ball>();
		Item.shootSpeed = 4.5f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		switch (Main.rand.Next(2))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.Introductiontothermalelectricity_launch_1_Mod), position);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.Introductiontothermalelectricity_launch_2_Mod), position);
				break;
		}

		Projectile.NewProjectileDirect(source, position + velocity * 4, velocity, type, damage, knockback, player.whoAmI);
		return false;
	}
}
