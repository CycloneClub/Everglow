using Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class VFXPersonItem : ModItem
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
		for (int k = 0; k < 10; k++)
		{
			YggdrasilTownPersonManager.AddRandomPerson();
		}
		return false;
	}
}
