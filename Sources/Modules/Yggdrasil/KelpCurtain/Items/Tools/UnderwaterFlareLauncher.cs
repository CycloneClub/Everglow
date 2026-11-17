using Everglow.Yggdrasil.KelpCurtain.Projectiles.Tools;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools;

public class UnderwaterFlareLauncher : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public int CoolTimer = 0;

	public override void SetDefaults()
	{
		Item.width = 72;
		Item.height = 24;

		Item.useTime = Item.useAnimation = 60;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.UseSound = SoundID.Item108;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Orange;
		Item.value = 20000;

		Item.shoot = ModContent.ProjectileType<UnderwaterFlareLauncher_Proj>();
		Item.shootSpeed = 12f;
	}

	public override void HoldItem(Player player) => base.HoldItem(player);

	public override bool? UseItem(Player player) => base.UseItem(player);

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		type = ModContent.ProjectileType<UnderwaterFlareLauncher_Proj>();
		position += velocity * 4;
	}
}
