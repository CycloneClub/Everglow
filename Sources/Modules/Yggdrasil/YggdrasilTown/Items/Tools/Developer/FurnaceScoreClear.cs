using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class FurnaceScoreClear : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void HoldItem(Player player)
	{
		base.HoldItem(player);
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			player.GetModPlayer<FurnacePlayer>().TotalFurnaceScore = 0;
			player.GetModPlayer<FurnacePlayer>().CurrentFurnaceScore = 0;
		}
	}
}
