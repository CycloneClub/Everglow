using Everglow.Yggdrasil.GreenCore.Projectiles.Melee;

namespace Everglow.Yggdrasil.GreenCore.Items.Weapons;

public class CharonDoubleSickle_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.damage = 100;

		Item.width = 112;
		Item.height = 106;
		Item.useTime = 3;
		Item.useAnimation = 15;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.autoReuse = true;
		Item.channel = true;
		Item.useLimitPerAnimation = 2;
		Item.shoot = ModContent.ProjectileType<CharonDoubleSickle>();
		Item.DamageType = DamageClass.MeleeNoSpeed;
	}

	public override void HoldItem(Player player) => player.ListenMouseWorld();

	public override bool CanUseItem(Player player)
	{
		return player.ownedProjectileCounts[Item.shoot] < 2;
	}
}
