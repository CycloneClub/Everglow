using Everglow.Minortopography.GiantPinetree.Projectiles;
using Terraria.Localization;

namespace Everglow.Minortopography.GiantPinetree.Items;

public class SnowPineLockBox : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public static readonly int ResourceBoost = 100;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ResourceBoost);

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 52;
		Item.value = 4090;
		Item.accessory = true;
		Item.rare = ItemRarityID.Blue;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<PineSprite>()] < 1)
		{
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.MountedCenter + new Vector2(-player.direction * 40, 0), player.velocity, ModContent.ProjectileType<PineSprite>(), 0, 0, Owner: player.whoAmI);
		}
	}
}
