using Terraria;
namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

public class GoodNeighborCoin : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Green;
		Item.value = 0;
	}

	public override bool CanStackInWorld(WorldItem destination, WorldItem source) => true;
}
