using SubworldLibrary;

namespace Everglow.Yggdrasil;

public class YggdrasilImpression : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.width = 50;
		Item.height = 50;
		Item.useTurn = true;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = false;
	}

	public override bool? UseItem(Player player)
	{
		// TODO world
		if (player.itemAnimation == player.itemAnimationMax)
		{
			Ins.VFXManager.Clear();
			if (SubworldSystem.IsActive<YggdrasilWorld>())
			{
				SubworldSystem.Exit();
			}
			else
			{
				if (!SubworldSystem.Enter<YggdrasilWorld>())
				{
					Main.NewText("Fail!");
				}
			}
		}

		// int projectileType = ModContent.ProjectileType<TeleportToYggdrasil>();
		// if (player.ownedProjectileCounts[projectileType] <= 0)
		// {
		// Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromAI(), player.Center, Vector2.zeroVector, projectileType, 0, 0, player.whoAmI);
		// proj.extraUpdates = 8;
		// }
		return false;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.Register();
	}
}
