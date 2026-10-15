using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class MagicalBoomerang : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 26;
		Item.knockBack = 1.0f;
		Item.crit = 4;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = Item.useTime = 36;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.rare = ItemRarityID.Blue;
		Item.value = 4000;

		Item.shoot = ModContent.ProjectileType<MagicalBoomerangProj>();
		Item.shootSpeed = 13;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<MagicalBoomerangProj>()] + player.ownedProjectileCounts[ModContent.ProjectileType<MagicalBoomerangSubProj>()] > 0)
		{
			return false;
		}
		return base.CanUseItem(player);
	}
}
