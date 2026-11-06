using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;

public class DeadBeetleEgg : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 40;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 28;
		Item.knockBack = 0.2f;
		Item.mana = 11;

		Item.useTime = Item.useAnimation = 26;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item117;
		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.channel = true;

		Item.value = 11700;
		Item.rare = ItemRarityID.Green;

		Item.shoot = ModContent.ProjectileType<DeadBeetleEgg_egg>();
		Item.shootSpeed = 0;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}
}
