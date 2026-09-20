using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class MetroHelper : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player)
	{
		ColliderManager.Instance.Add<YggdrasilTown_NormalMetro>(Main.MouseWorld);
		return false;
	}
}
