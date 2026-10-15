using Everglow.Commons.Mechanics.Quest.UI;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class TestOpenQuestPanelWithSelectQuestItem : ModItem
{
	public override string Texture => ModAsset.Point_Mod;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noUseGraphic = true;
	}

	public override bool? UseItem(Player player)
	{
		QuestContainer.Instance.ShowWithQuest(new OpenPanelQuestTest().Name);
		return true;
	}
}
