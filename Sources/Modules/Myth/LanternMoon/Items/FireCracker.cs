using Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

namespace Everglow.Myth.LanternMoon.Items;

public class FireCracker : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 50;
		Item.value = 10000;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			Projectile.NewProjectileDirect(player.GetSource_FromAI(), Main.MouseWorld, Vector2.zeroVector, ModContent.ProjectileType<Firework12Inches>(), 500, 2, player.whoAmI);
		}
	}
}
