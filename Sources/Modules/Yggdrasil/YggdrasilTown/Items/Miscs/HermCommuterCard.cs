using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

public sealed class HermCommuterCard : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 20;
		Item.maxStack = 1;
		Item.rare = ItemRarityID.Quest;
		Item.questItem = true;
	}
}
