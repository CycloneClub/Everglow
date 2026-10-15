namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

public class YggdrasilPlayerRoomDoorKey : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;

		Item.stack = Item.CommonMaxStack;

		Item.rare = ItemRarityID.White;
		Item.value = 60000;
	}

	public override bool CanStackInWorld(Item source) => true;
}
