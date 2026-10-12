using Everglow.Commons.Utilities;
using Everglow.Example.Projectiles;

namespace Everglow.Example.Items;

public class ExampleTrailingProjectile_Emit : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults() => base.SetDefaults();

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ExampleTrailingProjectile>()] <= 0)
			{
				Vector2 vel = Main.MouseWorld - player.Center;
				vel = vel.NormalizeSafe() * 15f;
				Projectile.NewProjectileDirect(player.GetSource_FromAI(), player.Center, vel, ModContent.ProjectileType<ExampleTrailingProjectile>(), 1000, 10, player.whoAmI);
			}
		}
		base.HoldItem(player);
	}
}
